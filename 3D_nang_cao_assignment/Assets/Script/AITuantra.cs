using UnityEngine;
using UnityEngine.AI;

public class AITuantra : MonoBehaviour
{
    [Header("Cài đặt Tuần tra")]
    [Tooltip("Tốc độ khi chạy vòng vòng tuần tra")]
    public float patrolSpeed = 3.5f;
    [Tooltip("Mục tiêu để đuổi theo (kéo Player vào đây)")]
    public Transform player;
    [Tooltip("Khoảng cách nhìn thấy Player để chuyển sang Đuổi theo")]
    public float chaseRange = 10f;
    [Tooltip("Các điểm để quái vật đi tuần tra qua lại")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    private NavMeshAgent agent;

    // Tham chiếu tới 2 script kia để bật/tắt
    private AIDuoiTheo scriptDuoiTheo;
    private AiTanCong scriptTanCong;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        scriptDuoiTheo = GetComponent<AIDuoiTheo>();
        scriptTanCong = GetComponent<AiTanCong>();
    }

    void OnEnable()
    {
        // Khi bật script này lên (hoặc quay lại từ đuổi theo), set tốc độ đi tuần chạy chậm lại
        if (agent != null)
        {
            agent.speed = patrolSpeed;
            agent.isStopped = false;
        }
    }

    void Start()
    {
        // Tự tìm Player nếu quên kéo vào
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Khi mới chạy game, đảm bảo chỉ có script Tuần Tra bật, 2 cái kia tắt
        if (scriptDuoiTheo != null) scriptDuoiTheo.enabled = false;
        if (scriptTanCong != null) scriptTanCong.enabled = false;

        // Đi tới điểm đầu tiên
        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void Update()
    {
        if (player == null) return;

        // Tính khoảng cách tới Player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 1. Nếu phát hiện Player trong tầm ngắm -> CHUYỂN SANG ĐUỔI THEO
        if (distanceToPlayer <= chaseRange)
        {
            if (scriptDuoiTheo != null && !scriptDuoiTheo.Equals(null))
            {
                Debug.Log("FSM: [Patrol] -> Chuyển sang [Chase]. Đang rượt theo Player!");
                scriptDuoiTheo.enabled = true; // Bật script đuổi theo
                this.enabled = false;          // Tắt script tuần tra này đi
                return;
            }
        }

        // 2. LOGIC TUẦN TRA: Đi theo đường đã vạch sẵn
        if (patrolPoints.Length == 0) return;

        agent.isStopped = false; // Đảm bảo quái vật được chạy

        // Nếu quái vật đi gần tới điểm đích (cách 0.5f) thì chuyển sang điểm tiếp theo
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length; // Đổi số thứ tự
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    // Vẽ vòng tròn màu vàng tượng trưng tầm nhìn
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
