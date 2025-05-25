using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class StartGameButton : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;

    GameState _gameState;

    [Inject]
    public void Construct(GameState gameState)
    {
        _gameState = gameState;
    }

    public void StartGame()
    {
        var t = inputField.text;
        if(t.Length > 0)
        {
            _gameState.LoadLevel(Convert.ToInt32(t));
        }
        else
        {
            _gameState.LoadLevel(0);
        }
    }

}
