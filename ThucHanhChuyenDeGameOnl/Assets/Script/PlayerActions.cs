using UnityEngine;
using Fusion;

public class PlayerActions : NetworkBehaviour
{
    private Animator animator;

    public override void Spawned()
    {
        // Lấy component Animator từ Game Object con chứa mô hình (Ví dụ: SM_FantasyFemale)
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        // Cần có Object == null vì Update() của Unity chạy liên tục kể cả trước khi máy chủ sinh nhân vật ra
        if (Object == null || !Object.HasInputAuthority) return;

        // BÀI 1: Bấm chuột trái để tấn công
        if (Input.GetMouseButtonDown(0))
        {
            RpcAttack();
        }

        // BÀI 2: Bấm Space để Nhảy và Phím F để Kỹ Năng
        // Note: Chức năng bay lên vật lý có thể đã code ở PlayerMovement, ở đây ta gọi RPC để đáp ứng yêu cầu đồng bộ âm thanh/thông báo của Lab
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RpcJump();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            RpcSpecialSkill();
        }
    }

    // [Rpc] đánh dấu đây là hàm đồng bộ mạng.
    // RpcSources.InputAuthority: Chỉ người điều khiển mới gọi được.
    // RpcTargets.All: Gửi và thực thi trên TẤT CẢ các client khác trong phòng.
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RpcAttack()
    {
        // Hiển thị thông báo trên Console theo yêu cầu của Bài 1
        Debug.Log($"[Client: {Runner.LocalPlayer}] Nhân vật {Object.Id} đang thực hiện Tấn công!");

        // Kích hoạt animation tấn công (cái biến Attack bạn vừa tạo trong Animator)
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RpcJump()
    {
        // Hiển thị thông báo Nhảy theo yêu cầu Bài 2
        Debug.Log($"[Client: {Runner.LocalPlayer}] Nhân vật {Object.Id} đang Nhảy!");
        // Animation nhảy thường quản lý bằng biến Bool ở ThirdPerson hoặc PlayerMovement rồi nên không gọi Trigger ở đây,
        // nếu sau này (Bài 3) có thêm tiếng bước nhảy, bạn sẽ gọi AudioSource.PlayOneShot() ở trong hàm này luôn!
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RpcSpecialSkill()
    {
        Debug.Log($"[Client: {Runner.LocalPlayer}] Nhân vật {Object.Id} đang dùng Kỹ năng đặc biệt (Phím F)!");
        
        if (animator != null)
        {
            // Tạm thời gọi trigger "Skill". Bạn có thể setup trong Animator tương tự như cách nối "Attack".
            animator.SetTrigger("Skill");
        }
    }
}
