using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TowerEditor : EditorWindow
{
    private List<GameObject> towerPrefabs = new List<GameObject>();
    private int selectedTowerIndex = -1;

    [MenuItem("TowerDefense/Tower Editor")]
    public static void ShowWindow()
    {
        GetWindow<TowerEditor>("Tower Editor");
    }

    private void OnGUI()
    {
        LoadTowersFromScene();

        EditorGUILayout.BeginHorizontal();
        
        EditorGUILayout.LabelField("Tower Prefabs", EditorStyles.boldLabel);
        
        if (GUILayout.Button("+", GUILayout.Width(20)))
        {
            towerPrefabs.Add(null);
        }

        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < towerPrefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            
            towerPrefabs[i] = (GameObject)EditorGUILayout.ObjectField
                (towerPrefabs[i], typeof(GameObject), false);
            
            if (GUILayout.Button("x", GUILayout.Width(20)))
            {
                towerPrefabs.RemoveAt(i);
                if (selectedTowerIndex == i)
                {
                    selectedTowerIndex = -1;
                }
            }

            if (GUILayout.Button("Edit", GUILayout.Width(50)))
            {
                selectedTowerIndex = i;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (towerPrefabs.Count > 0 && selectedTowerIndex >= 0 && selectedTowerIndex < towerPrefabs.Count &&
            towerPrefabs[selectedTowerIndex] != null)
        {
            Towers tower = towerPrefabs[selectedTowerIndex].GetComponent<Towers>();

            if (tower != null)
            {
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField
                ("Tower Properties (" + towerPrefabs[selectedTowerIndex].name + ")",
                    EditorStyles.boldLabel);
                
                EditorGUI.indentLevel++;

                tower.level = EditorGUILayout.IntField("Level", tower.level);
                
                tower.maxLevel = EditorGUILayout.IntField("Max Level", tower.maxLevel);
                
                tower.upgradeCost = EditorGUILayout.IntField("Upgrade Cost", tower.upgradeCost);

                tower.fireRate = EditorGUILayout.FloatField("Fire Rate", tower.fireRate);
                
                tower.range = EditorGUILayout.FloatField("Range", tower.range);
                
                tower.bulletPrefab = (GameObject)EditorGUILayout.ObjectField("Bullet Prefab", tower.bulletPrefab,
                    typeof(GameObject), false);

                // Add the next upgrade prefab field
                tower.nextUpgradePrefab = (GameObject)EditorGUILayout.ObjectField("Next Upgrade Prefab",
                    tower.nextUpgradePrefab, typeof(GameObject), false);

                EditorGUILayout.LabelField("Buildable Terrain Types", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                int newSize = EditorGUILayout.IntField("Size", tower.buildableTerrainTypes.Length);
                
                if (newSize != tower.buildableTerrainTypes.Length)
                {
                    Array.Resize(ref tower.buildableTerrainTypes, newSize);
                }

                for (int i = 0; i < tower.buildableTerrainTypes.Length; i++)
                {
                    tower.buildableTerrainTypes[i] =
                        EditorGUILayout.TextField($"Element {i}", tower.buildableTerrainTypes[i]);
                }

                EditorGUI.indentLevel--;

                EditorGUILayout.Space();
                
                if (GUILayout.Button("Save"))
                {
                    SaveTower(tower);
                }

            }
            else
            {
                EditorGUILayout.HelpBox
                    ("Selected prefab does not have a Towers component.", MessageType.Warning);
            }
        }
    }

    private void SaveTower(Towers tower)
    {
        if (tower != null)
        {
            EditorUtility.SetDirty(tower);
            AssetDatabase.SaveAssets();
        }
    }

    private void LoadTowersFromScene()
    {
        AvailableTowersInLevel availableTowersInLevel = FindObjectOfType<AvailableTowersInLevel>();
        if (availableTowersInLevel != null)
        {
            towerPrefabs = new List<GameObject>(availableTowersInLevel.towersInLevel);
        }
        else
        {
            EditorGUILayout.HelpBox("AvailableTowers script not found in the scene.", MessageType.Warning);
        }
    }
}