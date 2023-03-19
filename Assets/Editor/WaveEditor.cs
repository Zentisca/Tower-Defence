using UnityEngine;
using UnityEditor;

public class WaveEditor : EditorWindow
{
    //Testing Git!
    /// <summary>
    /// To DO LIST:
    /// Base.Enemy health/ Base.Enemy Speed
    /// Waves to complete round
    /// units to kill to complete round
    /// change Lives (cows)
    /// change starting gold
    /// change modifiers to unit per wave:
    /// 1.Health 2.Speed 3.ValueOnDeath
    ///
    /// Fix placement script to not allow buildings to be build inside one another
    /// Fix buildings to not be allowed to touch non "grass" terrain.Layers
    /// Fix when Left clicking buildings to return the button to normal not have to 2x press
    /// Fix Mouse drag of build Outline Prefab red, for when I click out of build Preview (works for green fine)
    ///
    /// Upgrade UI fix || overhaul system
    /// 
    /// Upgrade Tree / Skill Tree
    /// A toggle Camera Button for second Camera, this needs to also disable UI
    /// When in first person, allow walking, forward, back etc
    /// First person jump.
    /// </summary>
    
    
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
        // Get the reference to the EnemySpawner script
        enemySpawner = FindObjectOfType<EnemySpawner>();

        if (enemySpawner == null)
        {
            EditorGUILayout.LabelField("No EnemySpawner found in the scene.");
            return;
        }

        EditorGUILayout.LabelField("Spawning Settings", EditorStyles.boldLabel);
        enemySpawner.useRandomSpawn = EditorGUILayout.Toggle("Use Random Spawn", enemySpawner.useRandomSpawn);

        EditorGUILayout.LabelField("Wave Settings", EditorStyles.boldLabel);
        enemySpawner.enemiesPerWave = EditorGUILayout.IntField("Enemies per Wave", enemySpawner.enemiesPerWave);
        enemySpawner.Waves = EditorGUILayout.IntField("Total Waves", enemySpawner.Waves);

        EditorGUILayout.LabelField("Enemy Unit Settings", EditorStyles.boldLabel);
        enemySpawner.EnemiesPerWaveIncrement = EditorGUILayout.IntField("Enemies per Wave Increment", enemySpawner.EnemiesPerWaveIncrement);
        enemySpawner.SpeedIncrement = EditorGUILayout.FloatField("Speed Increment", enemySpawner.SpeedIncrement);
        enemySpawner.HealthIncrement = EditorGUILayout.IntField("Health Increment", enemySpawner.HealthIncrement);
        enemySpawner.GoldOnDeathIncrement = EditorGUILayout.IntField("Gold on Death Increment", enemySpawner.GoldOnDeathIncrement);
        
        if (enemySpawner.useRandomSpawn)
        {
            SerializedObject serializedObject = new SerializedObject(enemySpawner);
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("waypointsArray"), new GUIContent("Waypoints Array"), true);
            serializedObject.ApplyModifiedProperties();
        }
      
        else
        {
            enemySpawner.FixedWaypointSystem = (WaypointSystem)EditorGUILayout.ObjectField("Fixed Waypoint System", 
                enemySpawner.FixedWaypointSystem, typeof(WaypointSystem), true);
        }
        

        // Add other options as needed

        // Save the changes
        if (GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(enemySpawner);
            AssetDatabase.SaveAssets();
        }
    }
}
