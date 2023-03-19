using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TowerButton : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private TMP_Text towerCostText;
    private bool isTowerSelected;
    private Button button;
    private Player player;
    private Towers towers;
    
    public GameObject TowerPrefab
    {
        get { return towerPrefab; }
    }
    
    private void Start()
    {
        player = GameObject.Find("Player Elements").GetComponent<Player>();
        button = GetComponent<Button>();
        button.onClick.AddListener(ToggleTower);
        
        UpdateTowerCostText();
    }

    private void Update()
    {
        UpdateTowerCostText();
    }
    
    private void UpdateTowerCostText()
    {
        if (towerPrefab != null)
        {
            Towers towerComponent = towerPrefab.GetComponent<Towers>();
            if (towerComponent != null)
            {
                towerCostText.text = $"Cost: {towerComponent.towerCost}g";
            }
            else
            {
                towerCostText.text = "Cost: N/A";
            }
        }
        else
        {
            towerCostText.text = "Cost: N/A";
        }
    }

    private void ToggleTower()
    {
        if (isTowerSelected)
        {
            DeselectTower();
        }
        else
        {
            SelectTower();
        }
    }

    
    private void SelectTower()
    {
        if (towerPrefab != null)
        {
            Towers towers = towerPrefab.GetComponent<Towers>();
            int towerCost = Towers.GetTowerCost(towerPrefab);
            if (player.gold >= towerCost)
            {
                TowerPlacement.instance.selectedTower = towerPrefab;
                isTowerSelected = true;
                return;
            }
        }
        Debug.Log("Not Enough Gold for any towers!");
    }
    
    private void DeselectTower()
    {
        TowerPlacement.instance.selectedTower = null;
        isTowerSelected = false;
    }
}

