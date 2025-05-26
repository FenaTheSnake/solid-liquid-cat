using UnityEditor.SearchService;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] Transform _anchor;
    [SerializeField] RectTransform _text;

    Camera _cam;
    Canvas _canvas;

    bool _hidden = false;


    void Start()
    {
        _cam = Camera.main;
        _canvas = GameObject.FindAnyObjectByType<Canvas>();

        _text.transform.SetParent(_canvas.transform);
    }

    void Update()
    {
        if (_hidden) return;
        _text.position = _cam.WorldToScreenPoint(_anchor.position);
    }

    public void Show()
    {
        _text.gameObject.SetActive(true);
        _hidden = false;
    }

    public void Hide()
    {
        _text.gameObject.SetActive(false);
        _hidden = true;
    }
}
