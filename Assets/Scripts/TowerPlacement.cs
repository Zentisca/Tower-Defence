using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPlacement : MonoBehaviour
{
    public static TowerPlacement instance;
    public List<GameObject> towers = new List<GameObject>();
    private Towers _towers;
    public LayerMask towerBuildSlot;
    private GameObject _selectedTower;
    private Player player;
    [SerializeField] private GameObject greenTowerOutlinePrefab;
    private GameObject greenTowerInstance;
    private Terrain terrain;
    [SerializeField] private GameObject redTowerPrefab;
    private GameObject redTowerInstance;
    [SerializeField] private LayerMask buildableTerrainLayers;
    public List<string> buildableTerrainTextures;

    public GameObject selectedTower
    {
        get { return _selectedTower; }
        set
        {
            if (value == null && greenTowerInstance != null)
            {
                Destroy(greenTowerInstance);
            }

            _selectedTower = value;
        }
    }

    private void Start()
    {
        player = GameObject.Find("Player Elements").GetComponent<Player>();
        instance = this;
        terrain = Terrain.activeTerrain;
    }

    private void Update()
    {
        if (selectedTower != null)
        {
            FollowMouse();
            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUIObject())
                {
                    Debug.Log("IsPointerOverUIObject = returning");
                    return;
                }

                if (IsMouseOnValidTerrain())
                {
                    Debug.Log("Place Tower Logic running");
                    PlaceTower();
                }
            }
        }
    }

    private bool IsMouseOnValidTerrain()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, towerBuildSlot))
        {
            return CanBuildOnTerrainTexture(hit.point);
        }

        return false;
    }

    private void FollowMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, towerBuildSlot))
        {
            if (CanBuildOnTerrainTexture(hit.point))
            {
                if (greenTowerInstance == null)
                {
                    greenTowerInstance = Instantiate(greenTowerOutlinePrefab, hit.point, Quaternion.identity);
                }

                greenTowerInstance.transform.position = hit.point;

                if (redTowerInstance != null)
                {
                    Destroy(redTowerInstance);
                }
            }
            else
            {
                if (redTowerInstance == null)
                {
                    redTowerInstance = Instantiate(redTowerPrefab, hit.point, Quaternion.identity);
                }

                redTowerInstance.transform.position = hit.point;

                if (greenTowerInstance != null)
                {
                    Destroy(greenTowerInstance);
                }
            }
        }
    }

    private void PlaceTower()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, towerBuildSlot))
        {
            GameObject selectedTowerPrefab = GetSelectedTower();

            if (selectedTowerPrefab != null)
            {
                int towerCost = Towers.GetTowerCost(selectedTowerPrefab);
                if (player.gold >= towerCost)
                {
                    _towers = GetComponent<Towers>();
                    GameObject tower = Instantiate(selectedTowerPrefab, hit.point, Quaternion.identity);
                    towers.Add(tower);
                    player.gold -= towerCost;
                    selectedTower = null;

                    if (greenTowerInstance != null)
                    {
                        Destroy(greenTowerInstance);
                    }
                }
                else
                {
                    Debug.Log("Not Enough Gold for this tower!");
                }
            }
            else
            {
                Debug.Log("Unable to find the selected tower prefab.");
            }
        }
    }

    private GameObject GetSelectedTower()
    {
        foreach (TowerButton towerButton in FindObjectsOfType<TowerButton>())
        {
            if (towerButton.TowerPrefab == selectedTower) // Use the TowerPrefab property
            {
                return towerButton.TowerPrefab;
            }
        }

        return null;
    }


    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    private bool CanBuildOnTerrainTexture(Vector3 position)
    {
        Terrain terrain = Terrain.activeTerrain;
        Vector3 terrainPosition = terrain.transform.position;

        TerrainData terrainData = terrain.terrainData;
        TerrainLayer[] terrainLayers = terrainData.terrainLayers;

        int mapX = (int)(((position.x - terrainPosition.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)(((position.z - terrainPosition.z) / terrainData.size.z) * terrainData.alphamapHeight);

        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

        Towers tower = selectedTower.GetComponent<Towers>(); // Get the Tower component from the selected tower prefab

        for (int layerIndex = 0; layerIndex < terrainLayers.Length; layerIndex++)
        {
            if (splatmapData[0, 0, layerIndex] > 0.5f)
            {
                string textureName = terrainLayers[layerIndex].diffuseTexture.name;

                foreach (string buildableTerrainType in
                         tower.buildableTerrainTypes) // Access the buildable terrain types from the Tower script
                {
                    if (textureName.Contains(buildableTerrainType))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}