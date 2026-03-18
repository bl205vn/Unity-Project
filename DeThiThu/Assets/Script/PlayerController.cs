using UnityEngine;
using Unity.Cinemachine; // Thêm thư viện Cinemachine (Unity 6+)
using System.Collections; // Thêm thư viện Coroutine

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private bool isDead = false;

    [Header("Camera - Yêu cầu 1")]
    public CinemachineCamera gameplayCamera; // Kéo CinemachineCamera lúc chơi bình thường vào đây
    public CinemachineCamera deathCamera;    // Kéo CinemachineCamera góc nhìn nhân vật (tắt sẵn) vào đây

    [Header("Post Processing - Yêu cầu 4")]
    public GameObject blurVolume; // Kéo Game Object chứa Volume (Blur) vào đây

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (blurVolume != null) blurVolume.SetActive(false); // Tắt Blur ngay từ đầu
    }

    void Update()
    {
        if (isDead) return; // Nếu đã chết thì không cho di chuyển nữa

        // Nhận input di chuyển từ phím WASD hoặc mũi tên
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;
        
        // Di chuyển player
        rb.linearVelocity = new Vector3(move.x * moveSpeed, rb.linearVelocity.y, move.z * moveSpeed);
    }

    // Xử lý va chạm
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return; // Tránh gọi 2 lần
        isDead = true;
        Debug.Log("Player đã DIE! Chuyển góc nhìn Cinemachine Camera...");
        
        // Yêu cầu 1 - Chuyển cảnh sang góc nhìn nhân vật bằng Cinemachine (Đổi Priority)
        if (gameplayCamera != null) gameplayCamera.Priority = 0;   // Hạ mức độ ưu tiên của cam thường
        if (deathCamera != null) deathCamera.Priority = 10;        // Tăng mức độ ưu tiên của cam lúc chết để nó ghi đè lên
    }

    // --- YÊU CẦU 4: BỊ TRÚNG ĐẠN ---
    public void TakeDamage()
    {
        if (!isDead)
        {
            Debug.Log("Player trúng đạn! Bật màn hình Blur 1s");
            StartCoroutine(ShowBlurEffect());
        }
    }

    IEnumerator ShowBlurEffect()
    {
        // Bật Blur
        if (blurVolume != null) blurVolume.SetActive(true);
        
        // Chờ 1 giây
        yield return new WaitForSeconds(1f);
        
        // Tắt Blur
        if (blurVolume != null) blurVolume.SetActive(false);
    }
}
