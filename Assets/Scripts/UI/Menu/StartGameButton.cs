using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class StartGameButton : MonoBehaviour
{
    GameState _gameState;

    [Inject]
    public void Construct(GameState gameState)
    {
        _gameState = gameState;
    }

    public void StartGame()
    {
        _gameState.LoadLevel(0);
    }

}
