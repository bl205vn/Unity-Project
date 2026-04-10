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
}
