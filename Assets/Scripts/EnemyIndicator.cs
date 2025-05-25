using UnityEngine;

public enum EnemyIndicatorState
{
    HIDDEN,
    HMM,
    WARNING
}
public class EnemyIndicator : MonoBehaviour
{
    [SerializeField] Texture2D hmm;
    [SerializeField] Texture2D warning;
    [SerializeField] SoftbodyGenerator softbody;

    MeshRenderer meshRenderer;
    EnemyIndicatorState indicatorState = EnemyIndicatorState.HIDDEN;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        transform.parent = softbody.GetTrueTransform();
    }

    void Update()
    {
        //transform.position = Vector3.Lerp(transform.position, softbody.GetTrueTransform().position, 0.05f);
    }

    public void SetIndicator(EnemyIndicatorState newState)
    {
        if (newState == indicatorState) return;
        indicatorState = newState;

        if (indicatorState == EnemyIndicatorState.HIDDEN)
        {
            meshRenderer.enabled = false;
        }
        else if (indicatorState == EnemyIndicatorState.HMM)
        {
            meshRenderer.enabled = true;
            meshRenderer.material.mainTexture = hmm;
        }
        else if (indicatorState == EnemyIndicatorState.WARNING)
        {
            meshRenderer.enabled = true;
            meshRenderer.material.mainTexture = warning;
            //mat.mainTexture = warning;
        }
    }
}
