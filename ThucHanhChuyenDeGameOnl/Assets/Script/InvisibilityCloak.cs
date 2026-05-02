using UnityEngine;
using Fusion;

// CÂU 3: VẬT PHẨM ÁO CHOÀNG TÀNG HÌNH
// Hướng dẫn Setup trong Unity:
// 1. Tạo một 3D Object bất kỳ (Cube, Sphere...) làm vật phẩm, đặt tên "InvisibilityCloak"
// 2. Thêm Box Collider (tick Is Trigger) lên object đó
// 3. Gắn script này + Network Object vào
// 4. Chỉnh invisibilityDuration = 3 và respawnDelay = 15
public class InvisibilityCloak : NetworkBehaviour
{
    [Header("Cài đặt vật phẩm")]
    [SerializeField] private float invisibilityDuration = 3f;  // Tàng hình 3 giây
    [SerializeField] private float respawnDelay = 15f;         // Hồi sinh sau 15 giây

    // Trạng thái vật phẩm đồng bộ mạng: đang hiện hay đã bị nhặt
    [Networked, OnChangedRender(nameof(OnActiveChanged))]
    private NetworkBool IsActive { get; set; }

    // Đồng hồ đếm ngược để hồi sinh vật phẩm
    [Networked] private TickTimer RespawnTimer { get; set; }

    // Lưu lại các Renderer và Collider để bật/tắt hiển thị
    private Renderer[] renderers;
    private Collider triggerCollider;

    public override void Spawned()
    {
        renderers = GetComponentsInChildren<Renderer>();
        triggerCollider = GetComponent<Collider>();

        if (HasStateAuthority)
        {
            IsActive = true;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        // Nếu vật phẩm đang ẩn và hết thời gian chờ -> Hồi sinh
        if (!IsActive && RespawnTimer.Expired(Runner))
        {
            IsActive = true;
        }
    }

    // Khi Player chạm vào vật phẩm
    private void OnTriggerEnter(Collider other)
    {
        // Chỉ Host xử lý logic nhặt
        if (!HasStateAuthority) return;
        if (!IsActive) return;

        if (other.CompareTag("Player"))
        {
            // Tìm PlayerStats trên nhân vật vừa chạm
            PlayerStats stats = other.GetComponent<PlayerStats>();
            if (stats != null)
            {
                // Kích hoạt tàng hình cho người nhặt
                stats.ActivateInvisibility(invisibilityDuration);

                // Ẩn vật phẩm và bắt đầu đếm ngược hồi sinh
                IsActive = false;
                RespawnTimer = TickTimer.CreateFromSeconds(Runner, respawnDelay);
            }
        }
    }

    // Callback: Chạy trên MỌI CLIENT khi IsActive thay đổi
    private void OnActiveChanged()
    {
        // Bật/tắt hiển thị vật phẩm
        foreach (var r in renderers)
        {
            r.enabled = IsActive;
        }
        if (triggerCollider != null)
        {
            triggerCollider.enabled = IsActive;
        }
    }
}
