using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Cài đặt Đạn")]
    [Tooltip("Lượng máu trừ đi khi trúng quái")]
    public float damage = 20f;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 3f;

    private void Start()
    {
        // Tự động hủy sau x giây nếu không trúng gì để tránh rác game
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // Đạn luôn bay thẳng về phía trước theo trục Z của nó
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra xem viên đạn có chạm vào Enemy không
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Đã bắn trúng Enemy: " + other.gameObject.name);
            
            // Tìm script máu của con quái vừa bị bắn trúng
            AIHp enemyHp = other.GetComponentInParent<AIHp>();
            // Nếu không tìm thấy ở bản thân, mò tìm xem có script ở Parent hay con cái không (để tránh lỗi bạn đặt Collider ở GameObject con)
            if (enemyHp == null) enemyHp = other.GetComponentInChildren<AIHp>();
            if (enemyHp == null) enemyHp = other.GetComponent<AIHp>();

            if (enemyHp != null)
            {
                // Trừ máu dựa trên số sát thương bạn nhập ở Inspector
                enemyHp.TakeDamage(damage); 
            }
            
            // Hủy đạn khi trúng mục tiêu (đầu đạn nổ tung)
            Destroy(gameObject);
        }
        // Tránh đạn tự hủy (biến mất ngay lập tức) khi vô tình chạm vào bản thân Player lúc vừa bắn ra
        else if (!other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
