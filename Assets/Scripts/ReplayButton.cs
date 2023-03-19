using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReplayButton : MonoBehaviour
{
    public Button replayButton;
    private GameObject _gameObjectPlayerElements;
    private KeyBindings keyBindings;
    
    
    void Start()
    {
        _gameObjectPlayerElements = GameObject.Find("Player Elements");
        keyBindings = _gameObjectPlayerElements.GetComponent<KeyBindings>();
        
        replayButton.onClick.AddListener(RestartGame);
    }

    void RestartGame()
    {
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
            keyBindings.Resume();
        }
    }
}
