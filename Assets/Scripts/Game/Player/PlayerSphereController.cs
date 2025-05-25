using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerSphereController : PlayerController
{
    [SerializeField] Transform catFace;
    [SerializeField] Hand hand;
    [SerializeField] float catFaceOffset = 0.5f;

    [SerializeField] ParticleSystem particlesOnActivate;

    Rigidbody _rb;
    float _radius;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _radius = GetComponent<SphereCollider>().radius; 
    }

    public override GetAppropriatePositionResult GetAppropriatePositionForTransformation(Vector3 pos)
    {
        var levelLayer = LayerMask.GetMask("Level");

        bool checkAt = Physics.CheckSphere(pos, _radius, levelLayer);
        if (!checkAt) return new GetAppropriatePositionResult(true, pos);

        Collider[] cols = new Collider[8];
        Physics.OverlapSphereNonAlloc(pos, _radius, cols, levelLayer);

        Vector3 newPos = pos;
        foreach(Collider col in cols)
        {
            if (!col) continue;
            Vector3 p = Physics.ClosestPoint(newPos, col, col.transform.position, col.transform.rotation);
            //newPos = Vector3.MoveTowards(p, newPos, 1.0f);
            newPos = p + (newPos - p).normalized * (_radius * 1.1f);
        }

        checkAt = Physics.CheckSphere(newPos, _radius, levelLayer);
        if (!checkAt) return new GetAppropriatePositionResult(true, newPos);
        return new GetAppropriatePositionResult(false, pos);

        //Ray ray = new Ray(pos + Vector3.up, Vector3.down);
        //RaycastHit hit;
        //if(Physics.SphereCast(ray, 1.0f, out hit, 1.0f, LayerMask.GetMask("Level")))
        //{
        //    return new GetAppropriatePositionResult(true, hit.point);
        //}
    }

    public override void Activate()
    {
        gameObject.SetActive(true);
        _rb = GetComponent<Rigidbody>();
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        if (particlesOnActivate) particlesOnActivate.Play();

        //catFace.GetComponent<LookAtConstraint>().enabled = false;
    }

    public override void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public override Vector3 GetLastFrameVelocity()
    {
        return _rb.linearVelocity;
    }

    public override Transform GetTransform()
    {
        return transform;
    }

    public override void OnGrabEnd(InputAction.CallbackContext context)
    {
        hand.OnGrabEnd();
    }

    public override void OnGrabStart(InputAction.CallbackContext context)
    {
        hand.OnGrabStart();
    }

    public override void SetVelocity(Vector3 velocity)
    {
        _rb.AddForce(velocity * 40.592f, ForceMode.Impulse);
    }

    public override void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    private void Update()
    {
        catFace.transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position, catFaceOffset);
        //catFace.transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, catFaceOffset);
        //catFace.transform.LookAt(transform.position + transform.forward * 2.0f, Vector3.up);
        //catFace.transform.localEulerAngles = new Vector3(catFace.transform.localEulerAngles.x + 90.0f, catFace.transform.localEulerAngles.y, catFace.transform.localEulerAngles.z);

        catFace.GetComponent<LookAtConstraint>().roll -= _rb.linearVelocity.x * 2.0f;// * -Mathf.Sign(_rb.linearVelocity.x);
        //catFace.transform.localRotation = transform.localRotation; 
    }
}
