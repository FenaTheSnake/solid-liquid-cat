using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class Player : MonoBehaviour
{
    [SerializeField] List<PlayerController> playerControllers;
    PlayerController playerController;
    int curPCIndex = 0;

    InputAction _grabAction;
    InputAction _moveAction;
    InputAction _stickAction;
    InputAction _resetAction;

    GameState _gameState;

    [Inject]
    public void Construct(GameState gameState)
    {
        _gameState = gameState;
    }

    void Start()
    {
        _grabAction = InputSystem.actions.FindAction("Grab");
        _grabAction.started += OnGrabStart;
        _grabAction.canceled += OnGrabEnd;

        _stickAction = InputSystem.actions.FindAction("Stick");
        _stickAction.started += OnTransformation;

        _moveAction = InputSystem.actions.FindAction("Move");

        _resetAction = InputSystem.actions.FindAction("Reset");
        _resetAction.started += OnReset;

        playerController = playerControllers[curPCIndex];
    }

    private void OnDestroy()
    {
        _grabAction.started -= OnGrabStart;
        _grabAction.canceled -= OnGrabEnd;
        _stickAction.started -= OnTransformation;
        _resetAction.started -= OnReset;
    }

    void Update()
    {
        
    }

    public void OnGrabStart(InputAction.CallbackContext context)
    {
        playerController.OnGrabStart(context);
    }

    public void OnGrabEnd(InputAction.CallbackContext context)
    {
        playerController.OnGrabEnd(context);
    }

    public void OnTransformation(InputAction.CallbackContext context)
    {
        if (playerControllers.Count <= 1) return;

        var newPCIndex = (curPCIndex + 1) % playerControllers.Count;

        Vector3 vel = playerController.GetLastFrameVelocity();
        Vector3 pos = playerController.GetTransform().position;

        GetAppropriatePositionResult result = playerControllers[newPCIndex].GetAppropriatePositionForTransformation(pos);
        if (!result.canBePlaced) return;

        curPCIndex = newPCIndex;
        playerController.Deactivate();

        playerController = playerControllers[curPCIndex];
        playerController.Activate();
        playerController.SetVelocity(vel);
        playerController.SetPosition(result.pos);
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        _gameState.RestartLevel();
    }

    public Transform GetTransform()
    {
        return playerController.GetTransform();
    }
}
