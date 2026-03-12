using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Ссылки")]
    public Transform player;
    public Transform[] patrolPoints;

    [Header("Параметры")]
    public float chaseRange = 10f;
    public float attackRange = 2f;
    public int damage = 10;

    private NavMeshAgent agent;
    private int currentPatrolIndex = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= chaseRange)
        {
            agent.SetDestination(player.position);

            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void Attack()
    {
        Debug.Log("Враг атакует игрока! Урон: " + damage);
    }
}