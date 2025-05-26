using UnityEngine;
using Zenject;

public class WeakPlatform : MonoBehaviour
{
    bool isSolidCat;
    GameState _gameState;

    [Inject]
    void Construct(GameState gameState)
    {
        _gameState = gameState;
        _gameState.OnPlayerTransformation += OnTransformation;
    }

    void OnTransformation(PlayerController playerController)
    {
        if (playerController is PlayerSoftbodyController) isSolidCat = false;
        else if (playerController is PlayerSphereController) isSolidCat = true;
    }

    private void OnDestroy()
    {
        _gameState.OnPlayerTransformation -= OnTransformation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Player") && isSolidCat)
        {
            Destroy(gameObject);
        }
    }
}
