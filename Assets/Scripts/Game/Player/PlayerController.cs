using UnityEngine;
using UnityEngine.InputSystem;

public struct GetAppropriatePositionResult
{
    public bool canBePlaced;
    public Vector3 pos;
    public GetAppropriatePositionResult(bool canBePlaced, Vector3 pos) { this.canBePlaced = canBePlaced; this.pos = pos; }
}

public abstract class PlayerController : MonoBehaviour
{
    public abstract void OnGrabStart(InputAction.CallbackContext context);
    public abstract void OnGrabEnd(InputAction.CallbackContext context);
    public abstract GetAppropriatePositionResult GetAppropriatePositionForTransformation(Vector3 pos);
    public abstract void Activate();
    public abstract void Deactivate();

    public abstract Vector3 GetLastFrameVelocity();
    public abstract void SetVelocity(Vector3 velocity);
    public abstract void SetPosition(Vector3 pos);
    public abstract Transform GetTransform();

}
