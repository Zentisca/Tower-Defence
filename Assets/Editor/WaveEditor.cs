using UnityEngine;
using UnityEditor;

public class WaveEditor : EditorWindow
{
    private EnemySpawner enemySpawner;
    public bool useRandomSpawn;
    public GameObject[] waypointsArray;

    [MenuItem("TowerDefense/WaveEditor")]
    public static void ShowWindow()
    {
        GetWindow<WaveEditor>("Wave Editor");
    }

    private void OnGUI()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();

        if (enemySpawner == null)
        {
            EditorGUILayout.LabelField("No EnemySpawner found in the scene.");
            return;
        }

        EditorGUILayout.LabelField("Spawning Settings", EditorStyles.boldLabel);
        
        enemySpawner.useRandomSpawn = EditorGUILayout.Toggle
            ("Use Random Spawn", enemySpawner.useRandomSpawn);

        EditorGUILayout.LabelField("Wave Settings", EditorStyles.boldLabel);
        
        enemySpawner.enemiesPerWave = EditorGUILayout.IntField
            ("Enemies per Wave", enemySpawner.enemiesPerWave);
        
        enemySpawner.Waves = EditorGUILayout.IntField
            ("Total Waves", enemySpawner.Waves);

        EditorGUILayout.LabelField("Enemy Unit Settings", EditorStyles.boldLabel);
        
        enemySpawner.EnemiesPerWaveIncrement = EditorGUILayout.IntField
            ("Enemies per Wave Increment", enemySpawner.EnemiesPerWaveIncrement);
        
        enemySpawner.SpeedIncrement = EditorGUILayout.FloatField
            ("Speed Increment", enemySpawner.SpeedIncrement);
        
        enemySpawner.HealthIncrement = EditorGUILayout.IntField
            ("Health Increment", enemySpawner.HealthIncrement);
        
        enemySpawner.GoldOnDeathIncrement = EditorGUILayout.IntField
            ("Gold on Death Increment", enemySpawner.GoldOnDeathIncrement);
        
        if (enemySpawner.useRandomSpawn)
        {
            SerializedObject serializedObject = new SerializedObject(enemySpawner);
            
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty
                ("waypointsArray"), new GUIContent("Waypoints Array"), true);
            
            serializedObject.ApplyModifiedProperties();
        }
      
        else
        {
            enemySpawner.FixedWaypointSystem = (WaypointSystem)EditorGUILayout.ObjectField("Fixed Waypoint System", 
                enemySpawner.FixedWaypointSystem, typeof(WaypointSystem), true);
        }
        

        // Save the changes
        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(enemySpawner);
            AssetDatabase.SaveAssets();
        }
    }
}
