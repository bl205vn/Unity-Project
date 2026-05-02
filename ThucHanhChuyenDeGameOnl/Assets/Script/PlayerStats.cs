using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro; // Bắt buộc thêm dòng này để dùng được TextMeshPro (Điểm số)

public class PlayerStats : NetworkBehaviour
{
    // Khai báo Slider để kéo thả thanh máu trên đầu vào
    public Slider hpSlider;
    public Slider mpSlider;

    [Networked] public int HP { get; set; }
    [Networked] public int MP { get; set; }
    [Networked] public int Score { get; set; } // Bài 2: Biến điểm số mạng

    // ===== CÂU 3: ÁO CHOÀNG TÀNG HÌNH =====
    [Networked, OnChangedRender(nameof(OnInvisibleChanged))]
    public NetworkBool IsInvisible { get; set; }

    [Networked] private TickTimer InvisibilityTimer { get; set; }

    public override void Spawned()
    {
        // Chỉ Server/Host mới được quyền khởi tạo giá trị gốc
        if (Object.HasStateAuthority)
        {
            HP = 100;
            MP = 50;
            Score = 0;
        }
    }

    private void Update()
    {
        if (Object == null) return;

        // Bắt sự kiện phím (nhưng bỏ qua nếu đang chat!)
        if (Object.HasInputAuthority && !ChatUI.IsChatting)
        {
            if (Input.GetKeyDown(KeyCode.H)) RpcModifyStats(-10, 0, 0); // Bài 1: Trừ máu
            if (Input.GetKeyDown(KeyCode.J)) RpcModifyStats(0, -5, 0);  
            if (Input.GetKeyDown(KeyCode.P)) RpcModifyStats(0, 0, 1);  // Yêu cầu đề bài: +1 điểm
        }

        // Gán tiến độ cho Slider trên đầu nhân vật
        if (hpSlider != null) hpSlider.value = (float)HP / 100f;
        if (mpSlider != null) mpSlider.value = (float)MP / 50f;
    }

    public override void FixedUpdateNetwork()
    {
        // Kiểm tra hết thời gian tàng hình chưa (chỉ Host xử lý)
        if (HasStateAuthority && IsInvisible && InvisibilityTimer.Expired(Runner))
        {
            IsInvisible = false;
        }
    }

    // Hàm được InvisibilityCloak gọi khi Player nhặt vật phẩm
    public void ActivateInvisibility(float duration)
    {
        if (!HasStateAuthority) return;
        IsInvisible = true;
        InvisibilityTimer = TickTimer.CreateFromSeconds(Runner, duration);
    }

    // === CALLBACK: Tự động chạy trên MỌI CLIENT khi biến IsInvisible thay đổi ===
    private void OnInvisibleChanged()
    {
        // Tìm tất cả Renderer con (SkinnedMeshRenderer cho nhân vật, MeshRenderer cho phụ kiện)
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            r.enabled = !IsInvisible;
        }
    }

    // CỰC KỲ QUAN TRỌNG: Client KHÔNG THỂ tự tiện đổi biến [Networked]. Phải nhờ Server đổi hộ thông qua RPC
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcModifyStats(int hpChange, int mpChange, int scoreChange)
    {
        HP += hpChange;
        MP += mpChange;
        
        if (scoreChange > 0)
        {
            Score += scoreChange;
            // Dùng luôn ChatUI làm bảng điểm mini: Phát thông báo khi có người tăng điểm
            if (ChatManager.Instance != null)
            {
                ChatManager.Instance.RpcReceiveChatMessage("HỆ THỐNG", $"Player {Object.InputAuthority} vừa nhận {scoreChange} điểm! (Tổng: <color=yellow>{Score}</color> điểm)");
            }
        }
    }

    // --- SHARED MODE LOGIC ---
    // Hàm nhận sát thương (Ai cũng có thể tát mình -> RpcSources.All. Nhưng chỉ CƠ THỂ MÌNH (StateAuthority) mới được tụt máu)
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcTakeDamage(int damage, PlayerRef attacker)
    {
        HP -= damage;
        if (HP <= 0)
        {
            HP = 100; // Hồi sinh múa cột lại 100
            
            // Tìm và gửi tiền thưởng (điểm) ngược lại cho thủ phạm
            foreach (var player in FindObjectsByType<PlayerStats>(FindObjectsSortMode.None))
            {
                if (player.Object != null && player.Object.InputAuthority == attacker)
                {
                    player.RpcAddScore(1); // Yêu cầu đề bài: Hạ mục tiêu được +1 điểm
                    break;
                }
            }
        }
    }

    // Rpc để nhận tiền thưởng từ nạn nhân
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcAddScore(int points)
    {
        Score += points;
        // Dùng luôn ChatUI làm bảng điểm mini khi hạ gục mục tiêu
        if (ChatManager.Instance != null)
        {
            ChatManager.Instance.RpcReceiveChatMessage("BẢNG ĐIỂM", $"Player {Object.InputAuthority} vừa hạ mục tiêu! (Tổng: <color=yellow>{Score}</color> điểm)");
        }
    }
}
