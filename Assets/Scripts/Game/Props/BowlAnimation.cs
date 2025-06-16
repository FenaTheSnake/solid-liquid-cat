using UnityEngine;

public class BowlAnimation : MonoBehaviour
{
    Transform bowlGraphics;

    void Start()
    {
        bowlGraphics = transform.GetChild(0);
    }

    void Update()
    {
        transform.Rotate(new Vector3(0.0f, 0.15f, 0.0f));
        bowlGraphics.Rotate(new Vector3(Mathf.Sin(Time.fixedUnscaledTime * 1.47f) * 0.25f, 0, Mathf.Cos(Time.fixedUnscaledTime * 1.5f) * 0.13f));
    }
}
