using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] float runningSpeed = 3.5f;
    [SerializeField] float walkingSpeed = 1.25f;
    [SerializeField] float detectRadius = 5.0f;
    [SerializeField] float catchRadius = 0.75f;
    [SerializeField] SoftbodyGenerator player;
    [SerializeField] SoftbodyGenerator me;
    [SerializeField] float updateVertexEvery = 0.5f;
    [SerializeField] float movingStrength = 2.0f;
    [SerializeField] float vertsDragRadius = 0.4f;
    [SerializeField] float patrolDist = 1.5f;
    [SerializeField] Outline outline;

    [SerializeField] float lodRadius = 50.0f;
    bool unloaded = false;

    [SerializeField] EnemyIndicator indicator;

    Transform playerTransform;
    Transform meTransform;

    Vector3 idlePosition;
    Vector3 home;
    Vector3 pointOfInterest;

    NavMeshAgent agent;
    Rigidbody hookedVertex;
    bool following;

    float patrolTimer = 0.0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        playerTransform = player.GetTrueTransform();
        meTransform = me.GetTrueTransform();

        patrolTimer = Random.Range(1.0f, 5.0f);
        idlePosition = meTransform.position;
        home = meTransform.position;
    }

    //void UpdateHookedVertex()
    //{
    //    if (!following) return;

    //    if (timer < updateVertexEvery)
    //    {
    //        timer += Time.deltaTime;
    //        if (timer >= updateVertexEvery)
    //        {
    //            timer -= updateVertexEvery;

    //            var verts = me.phyisicedVertexes;

    //            Rigidbody rb = null;
    //            float mindist = 500.0f;
    //            foreach (var v in verts)
    //            {
    //                float d = Vector3.Distance(v.transform.position, agent.transform.position);
    //                if (d < mindist)
    //                {
    //                    mindist = d;
    //                    hookedVertex = v.GetComponent<Rigidbody>();
    //                }
    //            }
    //        }
    //    }
    //}

    void UpdatePatrol()
    {
        if (following) return;

        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0.0f)
        {
            patrolTimer = Random.Range(1.0f, 5.0f);

            Vector3 p = new Vector3(home.x + Random.Range(-patrolDist, patrolDist), 0, home.z + Random.Range(-patrolDist, patrolDist));
            NavMeshHit hit;
            if(NavMesh.SamplePosition(p, out hit, patrolDist * 3.0f, NavMesh.AllAreas))
            {
                idlePosition = hit.position;
            }
        }
    }

    void Update()
    {
        if (unloaded) return;
        //UpdateHookedVertex();
        UpdatePatrol();
        outline.ForceUpdateOutline();

        indicator.transform.position = Vector3.MoveTowards(meTransform.position + new Vector3(0.0f, 0.5f, 0.0f), Camera.main.transform.position, 0.7f);
    }

    private void FixedUpdate()
    {
        if(unloaded)
        {
            float distp = Vector3.Distance(transform.position, playerTransform.position);
            if(distp <= lodRadius)
            {
                unloaded = false;
            }
            return;
        }

        var verts = me.phyisicedVertexes;

        foreach (var v in verts)
        {
            float cad = Vector3.Distance(agent.transform.position, v.transform.position);
            if (cad > 0.5f)
            {
                Vector3 diff = agent.transform.position - v.transform.position;
                //agent.Move(-diff * Time.fixedDeltaTime * 15.0f);
                v.GetComponent<Rigidbody>().AddForce(diff * Time.fixedDeltaTime * 10.0f);
                //Debug.Log("diff " + diff);
            }
        }
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist <= detectRadius)
        {
            following = true;
            indicator.SetIndicator(EnemyIndicatorState.WARNING);
            pointOfInterest = playerTransform.position;
            agent.destination = pointOfInterest;
            agent.speed = runningSpeed;

            if (dist <= catchRadius)
            {
                //player.ApplyForce((player.GetComponent<SoftbodyController>().origin - playerTransform.position) * 0.05f + new Vector3(0, 0.25f, 0));
                Vector3 force = (playerTransform.position - transform.position) * 3.0f;
                me.ApplyForce(force);
                player.ApplyForce(force);
            }

        }
        else
        {
            if(following)
            {
                indicator.SetIndicator(EnemyIndicatorState.HMM);
                if (agent.remainingDistance <= 2.0f)
                {
                    agent.destination = idlePosition;
                    agent.speed = walkingSpeed;
                    following = false;
                    indicator.SetIndicator(EnemyIndicatorState.HIDDEN);
                }
            }
            else
            {
                agent.destination = idlePosition;
                agent.speed = walkingSpeed;
            }
        }

        if (hookedVertex != null)
        {
            //verts = me.phyisicedVertexes;

            foreach (var v in verts)
            {
                var d = Vector3.Distance(v.transform.position, hookedVertex.transform.position);

                if (d < vertsDragRadius)
                {
                    var factor = (vertsDragRadius - d) / vertsDragRadius;
                    Vector3 diff = agent.transform.position - hookedVertex.transform.position;
                    hookedVertex.AddForce(factor * movingStrength * Time.fixedDeltaTime * diff);
                    
                }
            }
        }

        if(dist > lodRadius)
        {
            unloaded = true;
        }
    }
}
