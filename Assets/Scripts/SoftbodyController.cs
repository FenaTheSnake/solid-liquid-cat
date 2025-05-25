using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SoftbodyController : MonoBehaviour
{
    [SerializeField] SoftbodyGenerator softbody;
    [SerializeField] float onCatGrabRadius = 200.0f;
    [SerializeField] float screenGrabRadius = 200.0f;
    [SerializeField] float movingRadius = 1.5f;
    [SerializeField] float strength = 1.5f;
    [SerializeField] float catFaceScale = 0.5f;
    [SerializeField] float catFaceOffset = 0.5f;
    [SerializeField] float jumpStrength = 2.0f;

    [SerializeField] Image cursorHoldImage;
    [SerializeField] Image catCenterImage;
    [SerializeField] Image catFace;
    [SerializeField] Transform catFace2;
    [SerializeField] Transform hand;
    [SerializeField] Transform dbgobj;
    [SerializeField] Transform stick;

    [SerializeField] List<SoftbodyGenerator> jumpableEnemies;

    public Vector3 origin;

    Collider holdCollider;
    Vector3 holdMousePosition = Vector3.zero;

    bool sticked = false;
    Vector3 stickPosition = Vector3.zero;
    List<SpringJoint> stickedVertices;

    InputAction grabAction;
    InputAction moveAction;
    InputAction stickAction;

    LayerMask levelMask;

    float _debugTimer = 0.0f;
    int calls = 0;

    Outline _outline;

    void Start()
    {
        grabAction = InputSystem.actions.FindAction("Grab");
        grabAction.started += OnGrabStart;
        grabAction.canceled += OnGrabEnd;

        stickAction = InputSystem.actions.FindAction("Stick");
        stickAction.started += OnStickStart;
        stickAction.canceled += OnStickEnd;

        moveAction = InputSystem.actions.FindAction("Move");


        levelMask = ~LayerMask.NameToLayer("Level");

        softbody = GetComponent<SoftbodyGenerator>();

        stickedVertices = new List<SpringJoint>();

        _outline = GetComponent<Outline>();

        origin = softbody.GetTrueTransform().position;
    }

    private void Update()
    {
        _outline.ForceUpdateOutline();
    }

    void OnStickStart(InputAction.CallbackContext context)
    {
        softbody.ApplyForce(Vector3.up * jumpStrength);

        //foreach(var e in jumpableEnemies)
        //{
        //    if (Vector3.Distance(e.GetTrueTransform().position, softbody.GetTrueTransform().position) < 5.0f)
        //    {
        //        e.ApplyForce(Vector3.up * jumpStrength);
        //        e.transform.position = e.transform.position + Vector3.up * jumpStrength * 2;
        //        foreach(var v in e.phyisicedVertexes)
        //        {
        //            v.transform.position -= Vector3.up * jumpStrength * 2;
        //        }
        //    }
        //}

        return;
        if (!sticked)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if (hit.collider.CompareTag("Vertex"))
                {
                    float vd = Vector3.Distance(hit.point, Camera.main.transform.position);

                    RaycastHit hit2;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit2, vd + 2.0f, levelMask))
                    {
                        stickPosition = hit.point;
                        sticked = true;
                        stick.transform.position = stickPosition;

                        var verts = softbody.phyisicedVertexes;
                        foreach (var v in verts)
                        {
                            var d = Vector3.Distance(v.transform.position, hit.collider.transform.position);
                            d = Mathf.Clamp(onCatGrabRadius - d, 0.0f, onCatGrabRadius);

                            if (d > 0.0f)
                            {
                                var sj = v.AddComponent<SpringJoint>();
                                sj.autoConfigureConnectedAnchor = false;
                                sj.connectedBody = stick.GetComponent<Rigidbody>();
                                sj.massScale = d;
                                stickedVertices.Add(sj);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            foreach (var sj in stickedVertices)
            {
                Destroy(sj);
            }
            stickedVertices.Clear();
            sticked = false;
            stickPosition = Vector3.zero;
            stick.transform.position = Vector3.zero;
        }
    }

    //void OnStickStart(InputAction.CallbackContext context)
    //{
    //    if (!sticked)
    //    {

    //        RaycastHit hit;
    //        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, levelMask))
    //        {
    //            stickPosition = hit.point;
    //            sticked = true;
    //            stick.transform.position = stickPosition;

    //            var verts = softbody.phyisicedVertexes;
    //            foreach (var v in verts)
    //            {
    //                var d = Vector3.Distance(v.transform.position, stickPosition);
    //                d = Mathf.Clamp(radius - d, 0.0f, radius);

    //                if (d > 0.0f)
    //                {
    //                    var sj = v.AddComponent<SpringJoint>();
    //                    sj.autoConfigureConnectedAnchor = false;
    //                    sj.connectedBody = stick.GetComponent<Rigidbody>();
    //                    sj.massScale = d;
    //                    stickedVertices.Add(sj);
    //                }
    //            }
    //        }

    //    }
    //    else
    //    {
    //        foreach(var sj in stickedVertices)
    //        {
    //            Destroy(sj);
    //        }
    //        stickedVertices.Clear();
    //        sticked = false;
    //        stickPosition = Vector3.zero;
    //        stick.transform.position = Vector3.zero;
    //    }

    //}

    void OnStickEnd(InputAction.CallbackContext context)
    {

    }

    void OnGrabStart(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, ~levelMask))
        {
            if (hit.collider.CompareTag("Vertex") && hit.collider.transform.parent == transform)
            {
                holdMousePosition = Input.mousePosition;

                //Vector2 movePos;
                //RectTransformUtility.ScreenPointToLocalPointInRectangle(
                //    cursorHoldImage.transform.parent.transform as RectTransform,
                //    Input.mousePosition, cursorHoldImage.transform.parent.GetComponent<Canvas>().worldCamera,
                //    out movePos);

                //cursorHoldImage.transform.position = cursorHoldImage.transform.parent.transform.TransformPoint(movePos);

                //RectTransformUtility.ScreenPointToLocalPointInRectangle(
                //    cursorHoldImage.transform.parent.transform as RectTransform,
                //    Camera.main.WorldToScreenPoint(GetComponent<SoftbodyGenerator>().GetTrueTransform().position),
                //    cursorHoldImage.transform.parent.GetComponent<Canvas>().worldCamera,
                //    out movePos);

                //catCenterImage.transform.position = catCenterImage.transform.parent.transform.TransformPoint(movePos);

                // //cursorHoldImage.rectTransform.localPosition = holdMousePosition;

                holdCollider = hit.collider;
            }
        }
    }
    void OnGrabEnd(InputAction.CallbackContext context)
    {
        holdMousePosition = Vector3.zero;
        holdCollider = null;
    }

    void ProcessGrabbing()
    {
        if (holdCollider == null) return;

        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cursorHoldImage.transform.parent.transform as RectTransform,
            Input.mousePosition, cursorHoldImage.transform.parent.GetComponent<Canvas>().worldCamera,
            out movePos);

        cursorHoldImage.transform.position = cursorHoldImage.transform.parent.transform.TransformPoint(movePos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cursorHoldImage.transform.parent.transform as RectTransform,
            Camera.main.WorldToScreenPoint(holdCollider.transform.position),
            cursorHoldImage.transform.parent.GetComponent<Canvas>().worldCamera,
            out movePos);

        catCenterImage.transform.position = catCenterImage.transform.parent.transform.TransformPoint(movePos);

        var holdColliderPoint = Camera.main.WorldToScreenPoint(holdCollider.transform.position);

        Vector3 diff = (Input.mousePosition - holdColliderPoint);
        float factor = Mathf.Clamp(diff.sqrMagnitude / screenGrabRadius, 0.0f, movingRadius);
        Vector3 to = holdCollider.transform.position + new Vector3(diff.normalized.x, 0, diff.normalized.y);

        Vector3 followPoint = Vector3.MoveTowards(holdCollider.transform.position, to, factor);
        hand.position = followPoint;

        var verts = holdCollider.transform.parent.GetComponent<SoftbodyGenerator>().phyisicedVertexes;

        foreach (var v in verts)
        {
            float d = Vector3.Distance(holdCollider.transform.position, v.transform.position);
            d = Mathf.Clamp(onCatGrabRadius - d, 0.0f, onCatGrabRadius);
            Vector3 f = (d / onCatGrabRadius) * strength * Time.fixedDeltaTime * (followPoint - v.transform.position);

            v.GetComponent<Rigidbody>().AddForce(f);
        }
    }

    void FixedUpdate()
    {
        catFace2.transform.position = Vector3.MoveTowards(softbody.GetTrueTransform().position, Camera.main.transform.position, catFaceOffset);
        ProcessGrabbing();
        //if (holdCollider != null)
        //{
        //    RaycastHit hit;
        //    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //    Vector3 followPoint = Vector3.zero;

        //    if(Physics.Raycast(ray, out hit, 100.0f, levelMask))
        //    {
        //        RaycastHit catRayHit;
        //        Vector3 dir = hit.point - holdCollider.transform.position;

        //        followPoint = Vector3.MoveTowards(holdCollider.transform.position, new Vector3(hit.point.x, holdCollider.transform.position.y, hit.point.z), movingRadius);
        //        dbgobj.transform.position = hit.point;
        //    }
        //    else
        //    {
        //        followPoint = holdCollider.transform.position;
        //    }
        //    hand.position = followPoint;

        //    var verts = holdCollider.transform.parent.GetComponent<SoftbodyGenerator>().phyisicedVertexes;

        //    foreach (var v in verts)
        //    {
        //        float d = Vector3.Distance(holdCollider.transform.position, v.transform.position);
        //        d = Mathf.Clamp(onCatGrabRadius - d, 0.0f, onCatGrabRadius);
        //        Vector3 f = (d / onCatGrabRadius) * strength * Time.fixedDeltaTime * (followPoint - v.transform.position);

        //        v.GetComponent<Rigidbody>().AddForce(f);
        //    }
        //}
    }
}
