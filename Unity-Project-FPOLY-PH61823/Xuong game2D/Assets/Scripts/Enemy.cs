using UnityEngine;

public class Enemy : MonoBehaviour
{
    // ------------------- SETTINGS -------------------
    [Header("Patrol Settings")]
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float patrolDistance = 0f;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private bool movingRight = true;

    // ------------------- CHASE -------------------
    [Header("Chase Settings")]
    [SerializeField] private float detectionRange = 0f;
    [SerializeField] private float chaseSpeed = 0f;
    [SerializeField] private float attackRange = 0f;
    [SerializeField] private Transform player;

    private bool isChasing = false;

    // ------------------- ATTACK -------------------
    [Header("Attack Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCooldown = 1f;

    private float lastAttackTime = -999f;

    private Health _health;

    // ------------------- UNITY METHODS -------------------
    private void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.right * patrolDistance;
        _health = GetComponent<Health>();


        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > attackRange * 1.5f)
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer(distanceToPlayer);
        }
        else
        {
            Patrol();
        }
    }

    // ------------------- PATROL LOGIC -------------------
    private void Patrol()
    {
        Vector3 destination = movingRight ? targetPosition : startPosition;

        transform.position = Vector2.MoveTowards(
            transform.position,
            destination,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, destination) < 0.1f)
        {
            movingRight = !movingRight;
        }

        FlipSprite(destination);
    }

    // ------------------- CHASE LOGIC -------------------
    private void ChasePlayer(float distanceToPlayer)
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            chaseSpeed * Time.deltaTime
        );

        FlipSprite(player.position);

        if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
    }

    // ------------------- ATTACK LOGIC -------------------
    private void Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;
        Debug.Log("Enemy attacks!");

        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null)
        {
            player.GetComponent<Health>()?.TakeDamage(damage); // Gây 10 damage

        }
    }

    public void TakeDamage(float damage)
    {
        _health?.TakeDamage(damage);
        // Gọi animation bị đánh nếu có
    }

    // ------------------- HELPER METHODS -------------------
    private void FlipSprite(Vector3 target)
    {
        if (target.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, targetPosition);
    }
}