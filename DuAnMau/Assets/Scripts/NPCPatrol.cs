using UnityEngine;

public class NPCPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform leftPoint; // Điểm bên trái
    [SerializeField] private Transform rightPoint; // Điểm bên phải
    [SerializeField] private float moveSpeed = 2f; // Tốc độ di chuyển
    [SerializeField] private float waitTime = 1f; // Thời gian chờ khi đến điểm
    
    [Header("Debug")]
    [SerializeField] private bool showGizmos = true; // Hiển thị gizmos trong editor
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool movingRight = true; // Hướng di chuyển hiện tại
    private bool isWaiting = false; // Đang chờ hay không
    private float waitTimer = 0f; // Đếm thời gian chờ
    
    void Start()
    {
        // Lấy components
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        // Kiểm tra và cảnh báo nếu thiếu components
        if (leftPoint == null || rightPoint == null)
        {
            Debug.LogError($"NPCPatrol on {gameObject.name}: Left Point or Right Point is not assigned!");
        }
        
        if (rb == null)
        {
            Debug.LogError($"NPCPatrol on {gameObject.name}: Rigidbody2D component is missing!");
        }
    }
    
    void Update()
    {
        if (leftPoint == null || rightPoint == null || rb == null) return;
        
        if (isWaiting)
        {
            // Đang chờ
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
                FlipDirection(); // Đổi hướng
            }
        }
        else
        {
            // Di chuyển
            Move();
        }
    }
    
    void Move()
    {
        Vector2 targetPosition = movingRight ? rightPoint.position : leftPoint.position;
        Vector2 currentPosition = transform.position;
        
        // Tính hướng di chuyển
        Vector2 direction = (targetPosition - currentPosition).normalized;
        
        // Di chuyển
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        
        // Cập nhật animation
        if (animator != null)
        {
            animator.SetBool("Run", true);
        }
        
        // Kiểm tra đã đến điểm đích chưa
        if (Mathf.Abs(currentPosition.x - targetPosition.x) < 0.05f)
        {
            // Đã đến điểm đích, bắt đầu chờ
            isWaiting = true;
            waitTimer = 0f;
            rb.linearVelocity = Vector2.zero; // Dừng di chuyển
            
            // Tắt animation
            if (animator != null)
            {
                animator.SetBool("Run", false);
            }
        }
    }
    
    void FlipDirection()
    {
        movingRight = !movingRight;
        
        // Lật sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !movingRight;
        }
    }
    
    // Phương thức để thay đổi tốc độ từ bên ngoài
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
    
    // Phương thức để thay đổi thời gian chờ từ bên ngoài
    public void SetWaitTime(float newWaitTime)
    {
        waitTime = newWaitTime;
    }
    
    // Phương thức để dừng patrol
    public void StopPatrol()
    {
        isWaiting = true;
        waitTimer = 0f;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        if (animator != null)
        {
            animator.SetBool("Run", false);
        }
    }
    
    // Phương thức để tiếp tục patrol
    public void ResumePatrol()
    {
        isWaiting = false;
        waitTimer = 0f;
    }
    
    // Vẽ gizmos trong editor để dễ setup
    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        
        if (leftPoint != null && rightPoint != null)
        {
            // Vẽ đường patrol
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftPoint.position, rightPoint.position);
            
            // Vẽ điểm bên trái
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(leftPoint.position, 0.3f);
            
            // Vẽ điểm bên phải
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(rightPoint.position, 0.3f);
        }
    }
} 