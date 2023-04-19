using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPlacement : MonoBehaviour
{
    public LayerMask towerBuildSlot;
    [SerializeField] private LayerMask buildableTerrainLayers;
    [SerializeField] private GameObject greenTowerOutlinePrefab;
    [SerializeField] private GameObject redTowerPrefab;
    public List<string> buildableTerrainTextures;
    public List<GameObject> towers = new List<GameObject>();

    public static TowerPlacement instance;
    private Terrain terrain;
    private Towers _towers;
    private Player player;
    private GameObject greenTowerInstance;
    private GameObject redTowerInstance;
    private GameObject _selectedTower;

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

    public GameObject overlapCheckObject;

    private void Start()
    {
        instance = this;
        player = GameObject.Find("Player Elements").GetComponent<Player>();
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
                    //Debug.Log("IsPointerOverUIObject = returning");
                    return;
                }

                if (IsMouseOnValidTerrain())
                {
                    //Debug.Log("Place Tower Logic running");
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
                    Debug.Log("Not Enough Gold for thistower!");
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

        bool canBuild = false;

        for (int layerIndex = 0; layerIndex < terrainLayers.Length; layerIndex++)
        {
            if (splatmapData[0, 0, layerIndex] > 0.5f)
            {
                string textureName = terrainLayers[layerIndex].diffuseTexture.name;
                
                Debug.Log("Checking buildable terrain types for tower: " + tower.name);
                foreach (string buildableTerrainType in tower.buildableTerrainTypes)
                {
                    Debug.Log("Allowed terrain type: " + buildableTerrainType);
                    if (textureName.Contains(buildableTerrainType))
                    {
                        canBuild = true;
                        break;
                    }
                    Debug.Log("Current terrain texture name: " + textureName);
                }

                if (canBuild)
                {
                    // Check for collider overlap
                    BoxCollider towerCollider = selectedTower.GetComponent<BoxCollider>();
                    if (towerCollider == null) return false; // No collider on the tower prefab

                    Vector3 towerSize = towerCollider.size;
                    Vector3 towerCenter = position + towerCollider.center;
                    Quaternion towerRotation = selectedTower.transform.rotation;

                    int resolution = 5; // Increase this value for more precise collider checks
                    float stepX = towerSize.x / resolution;
                    float stepZ = towerSize.z / resolution;

                    for (float x = towerCenter.x - towerSize.x * 0.5f;
                         x <= towerCenter.x + towerSize.x * 0.5f;
                         x += stepX)
                    {
                        for (float z = towerCenter.z - towerSize.z * 0.5f;
                             z <= towerCenter.z + towerSize.z * 0.5f;
                             z += stepZ)
                        {
                            Vector3 checkPoint = new Vector3(x, towerCenter.y, z);
                            int checkMapX = (int)(((checkPoint.x - terrainPosition.x) / terrainData.size.x) *
                                                  terrainData.alphamapWidth);
                            int checkMapZ = (int)(((checkPoint.z - terrainPosition.z) / terrainData.size.z) *
                                                  terrainData.alphamapHeight);
                            float[,,] checkSplatmapData = terrainData.GetAlphamaps(checkMapX, checkMapZ, 1, 1);

                            for (int checkLayerIndex = 0; checkLayerIndex < terrainLayers.Length; checkLayerIndex++)
                            {
                                if (checkSplatmapData[0, 0, checkLayerIndex] > 0.5f)
                                {
                                    string checkTextureName = terrainLayers[checkLayerIndex].diffuseTexture.name;
                                    bool foundBuildableType = false;
                                    foreach (string buildableTerrainType in tower.buildableTerrainTypes)
                                    {
                                        if (checkTextureName.Contains(buildableTerrainType))
                                        {
                                            foundBuildableType = true;
                                            break;
                                        }
                                    }
                                    if (!foundBuildableType)
                                    {
                                        return false; // Collider overlaps a non-buildable terrain type
                                    }
                                }
                            }
                        }
                    }

                    return true; // No collider overlap detected
                }
            }
        }

        return false;
    }
}