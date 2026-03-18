using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f; // Tốc độ đạn
    public float lifeTime = 5f; // Thời gian tồn tại trước khi tự hủy

    void Start()
    {
        // Tự động hủy viên đạn sau một khoảng thời gian để tránh rác game
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Yêu cầu 3: Đạn tự bay thẳng tới trước
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Nếu đạn trúng Player
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Yêu cầu 4: Hiệu ứng Blur toàn màn hình xuất hiện trong 1s (sẽ xử lý bên PlayerController)
                player.TakeDamage(); 
            }
            // Hủy viên đạn sau khi trúng Player
            Destroy(gameObject);
        }
        else if (other.CompareTag("Environment")) // Thêm Tag Gói môi trường nếu muốn đạn đụng tường thì nổ
        {
            Destroy(gameObject);
        }
    }
}
