using UnityEditor;
using UnityEngine;

public class GameEditor : EditorWindow
{
    private Player player;
    private int cows;
    private int gold;

    [MenuItem("TowerDefense/Game Properties")]
    public static void ShowWindow()
    {
        GetWindow<GameEditor>("Game Properties");
    }

    private void OnFocus()
    {
        UpdatePlayerReference();
    }

    private void UpdatePlayerReference()
    {
        player = GameObject.FindObjectOfType<Player>();
        if (player != null)
        {
            cows = player.cows;
            gold = player.gold;
        }
    }

    private void OnGUI()
    {
        if (player == null)
        {
            UpdatePlayerReference();
            if (player == null)
            {
                EditorGUILayout.HelpBox("Player script not found in the scene.", MessageType.Warning);
                return;
            }
        }

        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Player Properties", EditorStyles.boldLabel);

        // Display and edit cows
        cows = EditorGUILayout.IntField("Cows", cows);

        // Display and edit gold
        gold = EditorGUILayout.IntField("Gold", gold);

        EditorGUILayout.Space();

        // Save button
        if (GUILayout.Button("Save"))
        {
            SaveGameProperties();
        }

        EditorGUILayout.EndVertical();
    }

    private void SaveGameProperties()
    {
        player.cows = cows;
        player.gold = gold;
    }
}