using UnityEngine;
using TMPro; // Phải có thư viện này mới dùng được TextMeshPro con nha!

public class AIHp : MonoBehaviour
{
    [Header("Cài đặt Máu")]
    public float maxHp = 100f;
    private float currentHp;

    [Header("Giao diện (UI)")]
    [Tooltip("Kéo thả cái Text (TMP) trên đầu con quái vào ô này")]
    public TextMeshPro hpText;

    [Header("Âm thanh (Audio)")]
    [Tooltip("Kéo file âm thanh lúc quái chết vào đây")]
    public AudioClip deathSound;

    private void Start()
    {
        // Mới vào game máu đầy bình
        currentHp = maxHp;
        UpdateHpText();
    }

    private void Update()
    {
        // Dành cho UI 3D (Text nằm lơ lửng trên đầu): 
        // Làm cho chữ HP luôn quay mặt về phía hướng xoay của Camera để dễ đọc
        if (hpText != null && Camera.main != null)
        {
            hpText.transform.rotation = Camera.main.transform.rotation;
        }
    }

    /// <summary>
    /// Hàm dùng để nhận sát thương khi bị bắn trúng
    /// </summary>
    public void TakeDamage(float damageAmount)
    {
        currentHp -= damageAmount;
        
        // Không cho máu bị rớt xuống dưới số 0 (âm)
        if (currentHp < 0) currentHp = 0;

        UpdateHpText();

        // Kiểm tra xem đã chết chưa
        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void UpdateHpText()
    {
        if (hpText != null)
        {
            // Hiển thị số chẵn, thêm chữ HP cho xịn
            hpText.text = currentHp.ToString("0"); 
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " đã bị tiêu diệt!");

        // Phát âm thanh quái chết thông qua AudioManager bạn đã tạo lúc nãy
        if (AudioManager.Instance != null && deathSound != null)
        {
            AudioManager.Instance.PlaySFX(deathSound);
        }

        // (Tùy chọn nâng cao) Bạn có thể play Animation gục ngã hoặc rớt đồ ở đây
        
        // Trước mắt, quái vật sẽ biến mất khỏi bản đồ
        Destroy(gameObject);
    }
}
