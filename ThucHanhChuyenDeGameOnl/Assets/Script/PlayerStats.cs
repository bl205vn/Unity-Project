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
            if (Input.GetKeyDown(KeyCode.P)) RpcModifyStats(0, 0, 10);  // Bài 2: Nhấn P tăng 10 điểm
        }

        // Gán tiến độ cho Slider trên đầu nhân vật
        if (hpSlider != null) hpSlider.value = (float)HP / 100f;
        if (mpSlider != null) mpSlider.value = (float)MP / 50f;
    }

    // CỰC KỲ QUAN TRỌNG: Client KHÔNG THỂ tự tiện đổi biến [Networked]. Phải nhờ Server đổi hộ thông qua RPC
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcModifyStats(int hpChange, int mpChange, int scoreChange)
    {
        HP += hpChange;
        MP += mpChange;
        Score += scoreChange;
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
                    player.RpcAddScore(10);
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
    }
}
