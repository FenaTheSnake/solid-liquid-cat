using UnityEngine;

public class Bullet : MonoBehaviour
{
    float speed;
    Vector3 direction;

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void Shoot(Vector3 direction, float speed)
    {
        this.direction = direction;
        this.speed = speed;
    }
}
