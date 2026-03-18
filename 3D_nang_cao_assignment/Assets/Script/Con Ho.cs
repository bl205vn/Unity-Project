using UnityEngine;

public class ConHo : MonoBehaviour
{
    private enum State
    {
        Patrol,
        Alert
    }

    [SerializeField] private Transform[] patrolPoints = new Transform[3];
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float pointReachThreshold = 0.2f;

    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float lookSpeed = 5f;

    private State currentState = State.Patrol;
    private int currentPointIndex = 0;

    void Start()
    {
        if (player == null)
        {
            var p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Debug.LogWarning($"{nameof(ConHo)}: No patrol points assigned.");
            currentState = State.Alert;
        }
    }

    void Update()
    {
        if (player == null)
        {
            
            if (patrolPoints != null && patrolPoints.Length > 0) currentState = State.Patrol;
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
            currentState = State.Alert;
        else
            currentState = State.Patrol;

        switch (currentState)
        {
            case State.Patrol:
                PatrolUpdate();
                break;
            case State.Alert:
                AlertUpdate();
                break;
        }
    }

    private void PatrolUpdate()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPointIndex];
        if (target == null) return;

        
        Vector3 dir = (target.position - transform.position);
        Vector3 move = Vector3.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);
        transform.position = move;

        
        Vector3 flatDir = dir;
        flatDir.y = 0f;
        if (flatDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatDir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 8f * Time.deltaTime);
        }

        
        if (Vector3.Distance(transform.position, target.position) <= pointReachThreshold)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    private void AlertUpdate()
    {
        if (player == null) return;
        Vector3 dirToPlayer = player.position - transform.position;
        dirToPlayer.y = 0f;
        if (dirToPlayer.sqrMagnitude < 0.0001f) return;

        Quaternion lookRot = Quaternion.LookRotation(dirToPlayer.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, lookSpeed * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (patrolPoints != null)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                var p = patrolPoints[i];
                if (p == null) continue;
                Gizmos.DrawSphere(p.position, 0.1f);
                if (i + 1 < patrolPoints.Length && patrolPoints[i + 1] != null)
                    Gizmos.DrawLine(p.position, patrolPoints[i + 1].position);
            }

            if (patrolPoints.Length >= 2 && patrolPoints[0] != null && patrolPoints[patrolPoints.Length - 1] != null)
                Gizmos.DrawLine(patrolPoints[patrolPoints.Length - 1].position, patrolPoints[0].position);
        }
    }
}