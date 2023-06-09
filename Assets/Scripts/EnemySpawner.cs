using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public bool useRandomSpawn;
    public int enemiesPerWave = 10;
    public int totalWaves = 5;
    public int EnemiesPerWaveIncrement { get; set; } = 5;
    public float SpeedIncrement { get; set; } = 2f;
    public int HealthIncrement { get; set; } = 25;
    public int GoldOnDeathIncrement { get; set; } = 1;
    public Transform[] spawnPoints;
    public Transform[] fixedSpawnLocations;
    public GameObject[] waypointsArray;
    [SerializeField] public WaypointSystem fixedWaypointSystem;
    public GameObject enemyPrefab;
    [SerializeField] public float NextWaveSpawnTime = 5f;
    [SerializeField] public int enemiesKilled;
    [SerializeField] public int enemiesPassed;
    [SerializeField] public int currentWave = 1;
    [SerializeField] public float spawnTime = 3f;
    [SerializeField] private GameObject _gameObjectYouHaveWonPanel;
   
    
    private int enemiesSpawned;
    private int enemiesInScene;
    private int originalMaxHealth = 100;
    private int originalGoldOnDeath = 1;
    private float originalMoveSpeed = 2f;
    private bool gameOver;
    private EnemyUnit enemyUnit;

   
    public int Waves
    {
        get => totalWaves;
        set => totalWaves = value;
    }
    
   
    
   
    
    public WaypointSystem FixedWaypointSystem
    {
        get => fixedWaypointSystem;
        set => fixedWaypointSystem = value;
    }
    

    private void Start()
    {
        enemyUnit = enemyPrefab.GetComponent<EnemyUnit>();
        enemyUnit.agent.speed = originalMoveSpeed;
        enemyUnit.maxHealth = originalMaxHealth;
        enemyUnit.goldOnDeath = originalGoldOnDeath;

        if (currentWave > totalWaves && enemiesSpawned == enemiesPerWave && (enemiesSpawned - enemiesPassed) == enemiesKilled)
        {
            Spawn();
        }
        else
        {
            StartCoroutine(SpawnWaves());
        }
    }

    private void Update()
    {
        if (gameOver)
            return;

        if (currentWave == totalWaves && enemiesSpawned == enemiesPerWave) //Only looking for En
        {
            EndGame();
        }
    }

    
    IEnumerator SpawnWaves()
    {
        while (currentWave <= totalWaves)
        {
            if (enemiesSpawned < enemiesPerWave && enemiesInScene < enemiesPerWave)
            {
                Spawn();
                enemiesSpawned++;
                enemiesInScene++;
                yield return new WaitForSeconds(spawnTime);
            }
            else if (enemiesKilled == enemiesPerWave)
            {
                HandleNextWave();
                yield return new WaitForSeconds(5f); // 5 seconds delay before starting the next wave
                enemiesSpawned = 0; // Reset enemiesSpawned counter for the next wave
            }
            else
            {
                yield return null;
            }
        }
    }



    void Spawn()
    {
        WaypointSystem selectedWaypoints;
        Transform spawnTransform;

        if (useRandomSpawn)
        {
            int spawnPointIndex = Random.Range(0, spawnPoints.Length);
            selectedWaypoints = waypointsArray[Random.Range(0, waypointsArray.Length)].GetComponent<WaypointSystem>();
            spawnTransform = spawnPoints[spawnPointIndex];
        }
        else
        {
            int fixedSpawnIndex = Random.Range(0, fixedSpawnLocations.Length);
            selectedWaypoints = fixedWaypointSystem;
            spawnTransform = fixedSpawnLocations[fixedSpawnIndex];
        }

        GameObject enemy = Instantiate(enemyPrefab, spawnTransform.position, spawnTransform.rotation);
        EnemyUnit enemyUnitScript = enemy.GetComponent<EnemyUnit>();
        enemyUnitScript.AssignWaypoints(selectedWaypoints.GetWaypoints());
    }

    private void HandleNextWave()
    {
        enemiesPerWave += EnemiesPerWaveIncrement;
        enemyUnit.agent.speed += SpeedIncrement;
        enemyUnit.maxHealth += HealthIncrement;
        enemyUnit.goldOnDeath += GoldOnDeathIncrement;
        enemiesKilled = 0;
        enemiesSpawned = 0;
        enemiesPassed = 0;
        currentWave++;
        Debug.Log("Starting wave " + currentWave);
    }


    IEnumerator WaitForNextWave()
    {
        yield return new WaitForSeconds(NextWaveSpawnTime);
        StartCoroutine(SpawnWaves());
    }
    
    private void EndGame()
    {
        gameOver = true;
        StopCoroutine(SpawnWaves());
        _gameObjectYouHaveWonPanel.SetActive(true);
        Debug.Log("Congrats, you win!");
    }
    public void IncreaseKills()
    {
        enemiesKilled++;
        enemiesInScene--;
        Debug.Log("Enemies killed: " + enemiesKilled);
        if (enemiesKilled >= enemiesPerWave)
        {
            HandleNextWave();
        }
    }
    
    
    
    public void EnemyPassed()
    {
        enemiesPassed++;
        Debug.Log("Enemies passed: " + enemiesPassed);
        Spawn(); // Spawn a new enemy immediately
        enemiesInScene++;
    }


    IEnumerator SpawnEnemyWithDelay()
    {
        if (enemiesInScene < enemiesPerWave)
        {
            yield return new WaitForSeconds(1f);
            Spawn();
            enemiesInScene++;
        }
    }


    void ResetEnemyPrefab()
    {
        enemyUnit.agent.speed = originalMoveSpeed;
        enemyUnit.maxHealth = originalMaxHealth;
        enemyUnit.goldOnDeath = originalGoldOnDeath;
    }

    private void OnApplicationQuit()
    {
        ResetEnemyPrefab();
    }

}
