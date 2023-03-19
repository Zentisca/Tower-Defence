using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    public GameObject[] upgradedTowerPrefabs; // Array of tower prefabs for each level
    public Player player; // Reference to the player component that holds the gold variable

    private bool upgradeModeActive = false; // Flag to toggle the upgrade mode on and off

    private void Update()
    {
        // Check if the upgrade mode is active and the player has clicked on a tower
        if (upgradeModeActive && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object has a Towers component
                Towers towers = hit.collider.GetComponent<Towers>();

                if (towers != null)
                {
                    if (towers.level < towers.maxLevel)
                    {
                        // Check if the player has enough resources to upgrade
                        if (player.gold >= towers.upgradeCost)
                        {
                            // Remove the old tower
                            Destroy(towers.gameObject);

                            // Instantiate the new tower
                            Instantiate(towers.nextUpgradePrefab, towers.transform.position, towers.transform.rotation);

                            // Deduct the upgrade cost from the player's resources
                            player.gold -= towers.upgradeCost;
                        }
                        else
                        {
                            // Display a message indicating that the upgrade is not possible
                            Debug.Log("Not enough resources to upgrade the tower.");
                        }
                    }
                    else if (towers.level >= towers.maxLevel)
                    {
                        Debug.Log("The tower is already at its maximum level.");
                    }
                }
                else
                {
                    Debug.Log("The selected object does not have a Towers component.");
                }
            }
        }
    }

    public void ToggleUpgradeMode()
    {
        upgradeModeActive = !upgradeModeActive;

        // Display a message indicating that the upgrade mode has been toggled
        if (upgradeModeActive)
        {
            Debug.Log("Upgrade mode is now active.");
        }
        else
        {
            Debug.Log("Upgrade mode is now inactive.");
        }
    }
}