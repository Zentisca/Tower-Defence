using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text wavesRemainingText;
    public TMP_Text livesRemainingText;
    public TMP_Text playerGoldText;
    public TMP_Text killsReaminingText;
    
    private GameObject _gameObjectPlayer;
    private GameObject _gameObjectLevelWaves;
    private EnemySpawner enemySpawner;
    private Player player;

    private void Start()
    {
        _gameObjectPlayer = GameObject.Find("Player Elements");
        player = _gameObjectPlayer.GetComponent<Player>();
        _gameObjectLevelWaves = GameObject.Find("EnemySpawner");
        enemySpawner = _gameObjectLevelWaves.GetComponent<EnemySpawner>();

    }

    private void Update()
    {
        killsReaminingText.text =
            "Enemies Remaining: " + enemySpawner.enemiesKilled + "/" + enemySpawner.enemiesPerWave; 
        wavesRemainingText.text = "Waves Remaining: " + enemySpawner.currentWave + "/" + enemySpawner.totalWaves;  
        livesRemainingText.text = "Lives Remaining: " + player.cows;
        playerGoldText.text = "Current Gold: " + player.gold;
        //towerCostText.text = "Cost: " + towerButton.cost + "g";
    }
}
