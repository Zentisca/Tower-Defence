using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    public Player player;

    private bool upgradeModeActive = false;

    private void Update()
    {

        if (upgradeModeActive && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Towers towers = hit.collider.GetComponent<Towers>();

                if (towers != null)
                {
                    if (towers.level < towers.maxLevel)
                    {
                        if (player.gold >= towers.upgradeCost)
                        {
                            Destroy(towers.gameObject);
                            
                            Instantiate(towers.nextUpgradePrefab, towers.transform.position, towers.transform.rotation);
                            
                            player.gold -= towers.upgradeCost;
                        }
                        else
                        {
                            //Debug.Log("Not enough resources to upgrade the tower.");
                        }
                    }
                    else if (towers.level >= towers.maxLevel)
                    {
                        //Debug.Log("The tower is already at its maximum level.");
                    }
                }
                else
                {
                    //Debug.Log("The selected object does not have a Towers component.");
                }
            }
        }
    }

    public void ToggleUpgradeMode()
    {
        upgradeModeActive = !upgradeModeActive;
        
        if (upgradeModeActive)
        {
            //Debug.Log("Upgrade mode is now active.");
        }
        else
        {
            //Debug.Log("Upgrade mode is now inactive.");
        }
    }
}