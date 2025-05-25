using UnityEngine;
using Zenject;

public class Finish : MonoBehaviour
{
    GameState _gameState;
    bool _triggered = false;

    [Inject]
    public void Construct(GameState gameState)
    {
        _gameState = gameState;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if(other.CompareTag("Vertex"))
        {
            _gameState.LoadLevel(_gameState.Level + 1);
            _triggered = true;
        }
    }
}
