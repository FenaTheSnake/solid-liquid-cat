using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class StickySurface : MonoBehaviour
{
    List<Collider> _connectedColliders;
    List<CharacterJoint> _joints;
    Rigidbody _rb;

    Surfaces _surfaces;

    [Inject]
    public void Construct(Surfaces surfaces)
    {
        _surfaces = surfaces;

        _rb = GetComponent<Rigidbody>();
        _connectedColliders = new List<Collider>();
        _joints = new List<CharacterJoint>();

        _surfaces.RegisterStickySurface(this);
    }

    private void OnDestroy()
    {
        _surfaces.UnregisterStickySurface(this);
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Vertex") && !_connectedColliders.Contains(collision.collider))
        {
            var joint = collision.collider.AddComponent<CharacterJoint>();
            joint.connectedBody = _rb;
            _joints.Add(joint);
            _connectedColliders.Add(collision.collider);
        }
    }

    public void UnstickFromAllVertices()
    {
        for (int i = 0; i < _joints.Count; i++)
        {
            Destroy(_joints[i]);
        }
        _joints.Clear();
        _connectedColliders.Clear();
    }
}
