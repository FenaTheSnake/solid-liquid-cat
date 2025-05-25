using UnityEngine;

public class FloatingTextTrigger : MonoBehaviour
{
    FloatingText _floatingText;

    private void Start()
    {
        _floatingText = transform.parent.GetComponent<FloatingText>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _floatingText.Show();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _floatingText.Hide();
        }
    }
}
