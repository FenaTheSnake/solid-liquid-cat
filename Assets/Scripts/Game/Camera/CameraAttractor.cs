using UnityEngine;

public class CameraAttractor : MonoBehaviour
{
    Vector3 _floorPoint;
    GameCamera _camera;
    float _maxRadius;

    private void Awake()
    {
        _camera = Camera.main.GetComponent<GameCamera>();

        _maxRadius = CalculareCircumscribedCircle();


        var _levelMask = LayerMask.GetMask("Level");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100.0f, _levelMask))
        {
            _floorPoint = hit.point;
        } 
        else
        {
            Debug.LogWarning(name + " no floor found");
        }
    }

    float CalculareCircumscribedCircle()
    {
        var s = GetComponent<BoxCollider>().size;
        return Mathf.Sqrt(s.x * s.x + s.z * s.z) / 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        _camera.SetFollowing(_floorPoint);
        _camera.SetAnchorMaxRadius(_maxRadius);
    }
    private void OnTriggerExit(Collider other)
    {
        //_camera.SetFollowing(null);
    }
}
