using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] SoftbodyGenerator player;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float shootRadius = 5.0f;
    [SerializeField] public float shootSpeed = 0.5f;
    [SerializeField] public float accuracy = 0.75f;

    [SerializeField] float bulletSpeed = 0.5f;

    Transform playerTransform;
    float timer;

    public float projectileSpeed = 10f;
    public float arcHeight = 1f;

    public void ShootParabola(Vector3 targetPosition, bool isMiss)
    {
        GameObject projectile = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        Vector3 start = transform.position;
        Vector3 end = targetPosition;

        Vector3 direction = end - start;
        Vector3 horizontal = new Vector3(direction.x, 0f, direction.z);
        float distance = horizontal.magnitude;

        float heightDifference = end.y - start.y;
        float initialYVelocity = Mathf.Sqrt(-2f * Physics.gravity.y * arcHeight);
        float timeToPeak = initialYVelocity / -Physics.gravity.y;

        float totalTime = timeToPeak + Mathf.Sqrt(2f * (arcHeight - heightDifference) / -Physics.gravity.y) + Mathf.Epsilon;

        Vector3 horizontalVelocity = horizontal / totalTime;
        Vector3 velocity = horizontalVelocity + Vector3.up * initialYVelocity;

        rb.linearVelocity = velocity;

        projectile.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = isMiss ? Color.red : Color.yellow;
    }

    void Start()
    {
        player = GameObject.Find("Player").transform.GetChild(0).GetComponent<SoftbodyGenerator>();
        playerTransform = player.GetTrueTransform();
    }

    void ShootOrMiss()
    {
        var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Vector3 dir;

        if (Random.Range(0.0f, 1.0f) <= accuracy)   // shoot
        {
            dir = (playerTransform.position - transform.position).normalized;
        } 
        else // miss
        {
            Vector3 p = new Vector3(playerTransform.position.x + Random.Range(-1.0f, 1.0f), playerTransform.position.y + Random.Range(-1.0f, 1.0f), playerTransform.position.z + Random.Range(-1.0f, 1.0f));
            dir = (p - transform.position).normalized;
        }

        bullet.GetComponent<Bullet>().Shoot(dir, bulletSpeed);
    }

    void OnShootTimer()
    {
        timer += Time.deltaTime;
        if (timer < shootSpeed) return;

        timer -= shootSpeed;

        if (Random.Range(0.0f, 1.0f) <= accuracy)   // shoot
        {
            ShootParabola(playerTransform.position, false);
        }
        else // miss
        {
            float dist = Random.Range(2.0f, 3.5f);
            Vector3 dir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
            ShootParabola(playerTransform.position + dist * dir, true);
        }
    }

    void Update()
    {
        transform.LookAt(playerTransform.position, Vector3.up);
        float dist = Vector3.Distance(playerTransform.position, transform.position);

        if (dist <= shootRadius)
        {
            OnShootTimer();
        }
    }
}
