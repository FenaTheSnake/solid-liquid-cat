using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 0.1f;
    [SerializeField] Sprite handClosed;
    [SerializeField] Sprite handOpen;

    [HideInInspector] public RectTransform rectTransform;

    float realMouseSensitivity;

    bool holding;
    Vector3 holdCenter;
    float holdRadius = 0.0f;

    InputAction _moveAction;

    Image _image;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();

        _moveAction = InputSystem.actions.FindAction("Move");
        _moveAction.started += OnMove;

        Cursor.lockState = CursorLockMode.Locked;

        realMouseSensitivity = mouseSensitivity;
    }

    void OnDestroy()
    {
        _moveAction.started -= OnMove;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnGrabStart()
    {
        _image.sprite = handClosed;
        holding = true;
    }

    public void OnGrabEnd()
    {
        _image.sprite = handOpen;
        holding = false;
        realMouseSensitivity = mouseSensitivity;
        holdRadius = 0.0f;
    }

    public void LimitCursorToCircleThisFrame(Vector3 center, float radius)
    {
        holdCenter = center;
        holdRadius = radius;

        //var d = Vector3.Distance(center, rectTransform.position);
        //if (d > radius)
        //{
        //    rectTransform.position = Vector3.MoveTowards(center, rectTransform.position, radius);
        //    d = radius;
        //}

        //realMouseSensitivity = (1 - d / radius) * mouseSensitivity;
    }

    void OnMove(InputAction.CallbackContext context)
    {
        Vector3 prevPos = rectTransform.position;

        Vector2 moveValue = _moveAction.ReadValue<Vector2>() * realMouseSensitivity * 0.01f;

        if (holding && holdRadius > 0.0f)
        {
            float prevDist = Vector3.Distance(holdCenter, prevPos);
            float newDist = Vector3.Distance(holdCenter, prevPos + (Vector3)moveValue);

            float distDiff = newDist - prevDist;
            if (distDiff > 0)
            {
                distDiff *= Mathf.Clamp01(newDist / holdRadius);
            }
            else distDiff = 0;

            rectTransform.position = holdCenter + (((prevPos + (Vector3)moveValue) - holdCenter).normalized * (newDist - distDiff));
        }
        else
        {
            rectTransform.Translate(moveValue);
        }
    }

    private void Update()
    {
        if (holding && holdRadius > 0.0f)
        {
            float dist = Vector3.Distance(holdCenter, rectTransform.position);

            if(dist > holdRadius)
            {
                rectTransform.position = holdCenter + (rectTransform.position - holdCenter).normalized * holdRadius;
            }
        }
    }
}
