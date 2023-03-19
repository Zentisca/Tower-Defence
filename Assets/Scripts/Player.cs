using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]public int gold = 0; //the currency used in the game
    [SerializeField]public int cows = 0; //the amount of "lives" the player has
    [SerializeField]private int currentRoundLevel = 0; //at the round decided, player will win
    [SerializeField]private int endRoundLevel = 0;
    [SerializeField]private GameObject _gameObjectYouHaveWonPanel;
    [SerializeField]private GameObject _gameObjectYouHaveLostPanel; 
    
    private void Awake()
    {
        _gameObjectYouHaveWonPanel.SetActive(false);
        _gameObjectYouHaveLostPanel.SetActive(false);
    }

    private void Update()
    {
        HandleGameOver();
        HandleLevelComplete();
    }

    private void HandleGameOver()
    {
        //Game over when player reaches 0 Cows
        if (cows <= 0)
        {
            _gameObjectYouHaveLostPanel.SetActive(true);
            Debug.Log("Game Over, better luck next time! :)");
        }
    }

    private void HandleLevelComplete()
    {
        if (endRoundLevel == currentRoundLevel)
        {
            Debug.Log("Rejoice, the cows live! :D");
            _gameObjectYouHaveWonPanel.SetActive(true);
        }
    }

    public void LoseCow()
    {
        cows--;
    }
    public void AddGold(int value)
    {
        gold += value;
    }
    public void AddRound()
    {
        currentRoundLevel++;
    }
}
