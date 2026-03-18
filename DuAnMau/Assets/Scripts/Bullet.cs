using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed;        // Tốc độ đạn
    private float direction;    // Hướng di chuyển
    private float lifetime = 3f;  // Thời gian tồn tại của đạn
    private bool isDestroyed = false; // Đánh dấu đã bị hủy
    private Animator ani;

    private void Start()
    {
        // Tự động hủy đạn sau lifetime giây
        Destroy(gameObject, lifetime);
        ani = GetComponent<Animator>();
    }

    public void Initialize(float dir, float spd, float life)
    {
        direction = dir;
        speed = spd;
        lifetime = life;
        // Hủy đạn theo thời gian mới nếu được truyền vào sau khi Start
        CancelInvoke(); // Hủy mọi lệnh Destroy cũ
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Di chuyển đạn theo hướng và tốc độ
        if (!isDestroyed)
            transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroyed) return; // Nếu đã bị hủy thì không làm gì nữa

        // Kiểm tra va chạm với các object khác
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject); // Hủy đối tượng kẻ thù
            // Thêm kill count vào GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddKill();
            }
        }
        ani.SetTrigger("Danno");

        // Hủy đạn khi va chạm
        isDestroyed = true;
        Destroy(gameObject, 0.1f);
    }
} 