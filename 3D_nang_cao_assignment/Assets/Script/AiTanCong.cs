using UnityEngine;
using UnityEngine.AI;

public class AiTanCong : MonoBehaviour
{
    [Header("Cài đặt Tấn công")]
    public Transform player;
    [Tooltip("Khoảng cách nhìn thấy Player lùi ra để Đuổi theo lại")]
    public float attackRange = 2f;

    private NavMeshAgent agent;
    private Animator animator;

    // Tham chiếu tới script Đuổi theo
    private AIDuoiTheo scriptDuoiTheo;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        scriptDuoiTheo = GetComponent<AIDuoiTheo>();
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
        // Khi bắt đầu chuyển sang đánh, cho quái vật đứng lại
        if (agent != null)
        {
            agent.isStopped = true;
        }

        // Tùy chọn: Gọi animation đánh luôn khi vừa vào state này
        // if (animator != null) animator.SetTrigger("Attack");
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 1. CÓ RÕ RÀNG TRẠNG TRẠI: Player lùi ra khỏi tầm đánh -> TRỞ VỀ CHASE ĐỂ ĐUỔI TIẾP
        if (distanceToPlayer > attackRange)
        {
            if (scriptDuoiTheo != null && !scriptDuoiTheo.Equals(null))
            {
                Debug.Log("FSM: [Attack] -> Quay lại [Chase]. Player lùi ra, tiếp tục đuổi!");
                scriptDuoiTheo.enabled = true; // Bật lại script Đuổi Theo
                this.enabled = false;          // Tắt script Tấn công này đi
                return;
            }
        }

        // 2. LOGIC TẤN CÔNG: Đứng yên xoay mặt về phía Player
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Khóa trục Y để quái vật không bị ngửa mặt lên trời/xuống đất
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Xoay mượt
        }

        // Gợi ý: Chỗ này có thể viết thêm code để trừ máu Player hoặc chạy thời gian hồi đòn đánh (Cooldown)
    }

    // Vẽ vòng tròn màu đỏ tượng trưng tầm đánh
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
