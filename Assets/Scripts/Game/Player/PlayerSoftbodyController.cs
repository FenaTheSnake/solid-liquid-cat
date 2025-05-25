using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerSoftbodyController : PlayerController
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

    [SerializeField] ParticleSystem particlesOnActivate;

    SoftbodyGenerator _softbody;

    Collider _holdCollider;
    Vector3 _holdMousePosition = Vector3.zero;

    LayerMask _verticesMask;

    Vector3 lastPosition;
    Vector3 lastFrameVelocity;

    private void Start()
    {
        _verticesMask = LayerMask.GetMask("SoftbodyVertices");
        _softbody = GetComponent<SoftbodyGenerator>();
    }

    public override void OnGrabStart(InputAction.CallbackContext context)
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

    public override void OnGrabEnd(InputAction.CallbackContext context)
    {
        _holdMousePosition = Vector3.zero;
        _holdCollider = null;
        hand.OnGrabEnd();
    }

    public override GetAppropriatePositionResult GetAppropriatePositionForTransformation(Vector3 pos)
    {
        return new GetAppropriatePositionResult(true, pos);
    }

    public override void Activate()
    {
        foreach (var vert in _softbody.verticesRbs)
        {
            vert.linearVelocity = Vector3.zero;
        }

        catFace.GetComponent<LookAtConstraint>().roll = 0;

        if (particlesOnActivate)
        {
            particlesOnActivate.transform.parent = _softbody.GetTrueTransform();
            particlesOnActivate.transform.localPosition = Vector3.zero;
            particlesOnActivate.Play();
        }
        //gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        _holdMousePosition = Vector3.zero;
        _holdCollider = null;
        hand.OnGrabEnd();

        SetPosition(new Vector3(0, 0, -10000));
        //gameObject.SetActive(false); // can't disable cuz joints break (thx unity)
    }

    public override Vector3 GetLastFrameVelocity()
    {
        return lastFrameVelocity;
    }

    public override Transform GetTransform()
    {
        return _softbody.GetTrueTransform();
    }

    public override void SetPosition(Vector3 pos)
    {
        Vector3 diff = GetTransform().position - pos;
        foreach(var vert in _softbody.verticesRbs)
        {
            vert.MovePosition(vert.transform.position - diff);
        }
        _softbody.centerOfMasObj.transform.position = pos;
        //transform.position = pos;
    }

    public override void SetVelocity(Vector3 velocity)
    {
        _softbody.ApplyForce(velocity * 0.5f);
    }

    void ProcessGrabbing()
    {
        if (_holdCollider == null) return;

        var holdColliderPoint = Camera.main.WorldToScreenPoint(_holdCollider.transform.position);
        hand.LimitCursorToCircleThisFrame(holdColliderPoint, screenGrabRadius);

        Vector3 diff = (hand.rectTransform.position - holdColliderPoint);
        float factor = Mathf.Clamp(diff.sqrMagnitude / screenGrabRadius, 0.0f, movingRadius);
        float y = lastFrameVelocity.y * 10.0f;

        Vector3 movement = new Vector3(diff.normalized.x, 0, diff.normalized.y).normalized * movingRadius;
        movement *= Mathf.Clamp01(1 - (Mathf.Abs(y) / 9.81f));
        movement.y = y;
        Vector3 to = _holdCollider.transform.position + movement;

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

    private void Update()
    {
        catFace.transform.position = Vector3.MoveTowards(_softbody.GetTrueTransform().position, Camera.main.transform.position, catFaceOffset);
    }

    private void FixedUpdate()
    {
        lastFrameVelocity = _softbody.GetTrueTransform().position - lastPosition;
        lastPosition = _softbody.GetTrueTransform().position;
        ProcessGrabbing();
    }
}
