using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;

public class PlayerOld : MonoBehaviour
{
    [SerializeField] float onCatGrabRadius = 200.0f;
    [SerializeField] float screenGrabRadius = 200.0f;
    [SerializeField] float movingRadius = 1.5f;
    [SerializeField] float strength = 1.5f;
    [SerializeField] float catFaceScale = 0.5f;
    [SerializeField] float catFaceOffset = 0.5f;
    [SerializeField] float jumpStrength = 2.0f;

    [SerializeField] Transform catFace;
    [SerializeField] Hand hand;

    [SerializeField] GameObject sphereCat;

    SoftbodyGenerator _softbody;

    Collider _holdCollider;
    Vector3 _holdMousePosition = Vector3.zero;

    bool _sticked = false;
    Vector3 _stickPosition = Vector3.zero;
    List<SpringJoint> _stickedVertices;

    InputAction _grabAction;
    InputAction _moveAction;
    InputAction _stickAction;

    LayerMask _levelMask;
    LayerMask _verticesMask;

    Outline _outline;

    void Start()
    {
        _grabAction = InputSystem.actions.FindAction("Grab");
        _grabAction.started += OnGrabStart;
        _grabAction.canceled += OnGrabEnd;

        _stickAction = InputSystem.actions.FindAction("Stick");
        _stickAction.started += OnStickStart;

        _moveAction = InputSystem.actions.FindAction("Move");

        _levelMask = LayerMask.GetMask("Level");
        _verticesMask = LayerMask.GetMask("SoftbodyVertices");

        _softbody = GetComponentInChildren<SoftbodyGenerator>();

        _stickedVertices = new List<SpringJoint>();

        _outline = GetComponentInChildren<Outline>();
    }

    private void Update()
    {
        _outline.ForceUpdateOutline();
    }

    void OnStickStart(InputAction.CallbackContext context)
    {
        sphereCat.SetActive(true);
        sphereCat.transform.position = _softbody.GetTrueTransform().position;
        _softbody.gameObject.SetActive(false);
    }

    void OnGrabStart(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(hand.rectTransform.position), out hit, 100, _verticesMask.value))
        {
            if (_softbody.phyisicedVertexes.Contains(hit.collider.gameObject))
            {
                _holdMousePosition = hand.rectTransform.position;
                _holdCollider = hit.collider;

                hand.OnGrabStart();
            }
        }
    }
    void OnGrabEnd(InputAction.CallbackContext context)
    {
        _holdMousePosition = Vector3.zero;
        _holdCollider = null;
        hand.OnGrabEnd();
    }

    void ProcessGrabbing()
    {
        if (_holdCollider == null) return;

        var holdColliderPoint = Camera.main.WorldToScreenPoint(_holdCollider.transform.position);
        hand.LimitCursorToCircleThisFrame(holdColliderPoint, screenGrabRadius);

        Vector3 diff = (hand.rectTransform.position - holdColliderPoint);
        float factor = Mathf.Clamp(diff.sqrMagnitude / screenGrabRadius, 0.0f, movingRadius);
        Vector3 to = _holdCollider.transform.position + new Vector3(diff.normalized.x, 0, diff.normalized.y) * movingRadius;

        Vector3 followPoint = Vector3.MoveTowards(_holdCollider.transform.position, to, factor);

        var verts = _holdCollider.transform.parent.GetComponent<SoftbodyGenerator>().phyisicedVertexes;

        foreach (var v in verts)
        {
            float d = Vector3.Distance(_holdCollider.transform.position, v.transform.position);
            d = Mathf.Clamp(onCatGrabRadius - d, 0.0f, onCatGrabRadius);
            Vector3 f = (d / onCatGrabRadius) * strength * Time.fixedDeltaTime * (followPoint - v.transform.position);

            v.GetComponent<Rigidbody>().AddForce(f);
        }
    }

    void FixedUpdate()
    {
        catFace.transform.position = Vector3.MoveTowards(_softbody.GetTrueTransform().position, Camera.main.transform.position, catFaceOffset);
        ProcessGrabbing();
    }
}