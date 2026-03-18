using UnityEngine;
using UnityEngine.AI;

public class AIDuoiTheo : MonoBehaviour
{
    [Header("Cài đặt Đuổi Theo")]
    [Tooltip("Tốc độ khi phát hiện và rượt đuổi Player")]
    public float chaseSpeed = 6.5f;
    public Transform player;
    [Tooltip("Khoảng cách nhìn thấy Player để tiếp tục đuổi (thường bằng hoặc lớn hơn bên Tuần tra)")]
    public float chaseRange = 10f;
    [Tooltip("Khoảng cách bắt đầu Tấn công")]
    public float attackRange = 2f;

    private NavMeshAgent agent;

    // Tham chiếu tới 2 script kia để bật/tắt
    private AITuantra scriptTuantra;
    private AiTanCong scriptTanCong;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        scriptTuantra = GetComponent<AITuantra>();
        scriptTanCong = GetComponent<AiTanCong>();
    }

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void OnEnable()
    {
        // Mỗi khi script này được bật lên (do AITuantra gọi), đảm bảo NavMeshAgent cho phép chạy và set tốc độ dí
        if (agent != null)
        {
            agent.speed = chaseSpeed;
            agent.isStopped = false;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 1. CÓ RÕ RÀNG TRẠNG THÁI: Tới đủ gần -> CHUYỂN SANG ATTACK
        if (distanceToPlayer <= attackRange)
        {
            if (scriptTanCong != null && !scriptTanCong.Equals(null))
            {
                Debug.Log("FSM: [Chase] -> Chuyển sang [Attack]. Đang tấn công Player!");
                scriptTanCong.enabled = true; // Bật script tấn công
                this.enabled = false;         // Tắt script đuổi theo này đi
                return;
            }
        }

        // 2. CÓ RÕ RÀNG TRẠNG THÁI: Player chạy xa quá -> CHUYỂN VỀ PATROL
        if (distanceToPlayer > chaseRange)
        {
            if (scriptTuantra != null && !scriptTuantra.Equals(null))
            {
                Debug.Log("FSM: [Chase] -> Chuyển về [Patrol]. Player đã chạy xa, quay lại tuần tra!");
                scriptTuantra.enabled = true; // Bật lại script tuần tra
                this.enabled = false;         // Tắt script đuổi theo này đi
                return;
            }
        }

        // 3. LOGIC ĐUỔI THEO
        agent.isStopped = false; // Bỏ dừng
        agent.SetDestination(player.position); // Cập nhật đích đến liên tục về phía Player
    }
}
