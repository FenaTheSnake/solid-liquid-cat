using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody))]
public class HeavyBox : MonoBehaviour
{
    [SerializeField] float liquidCatMass = 50.0f;
    [SerializeField] float solidCatMass = 0.5f;

    Rigidbody _rb;
    GameState _gameState;

    [Inject]
    void Construct(GameState gameState)
    {
        _rb = GetComponent<Rigidbody>();
        _rb.mass = liquidCatMass;

        _gameState = gameState;
        _gameState.OnPlayerTransformation += OnTransformation;
    }

    void OnTransformation(PlayerController playerController)
    {
        if (playerController is PlayerSoftbodyController) _rb.mass = liquidCatMass;
        else if (playerController is PlayerSphereController) _rb.mass = solidCatMass;
    }

    private void OnDestroy()
    {
        _gameState.OnPlayerTransformation -= OnTransformation;
    }
}
