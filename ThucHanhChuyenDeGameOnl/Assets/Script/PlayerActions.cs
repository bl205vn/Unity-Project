using UnityEngine;
using Fusion;

public class PlayerActions : NetworkBehaviour
{
    private Animator animator;

    [Header("Âm thanh - Bài 3")]
    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip jumpSound;
    public AudioClip skillSound;
    public AudioClip footstepSound;

    public override void Spawned()
    {
        // Lấy component Animator từ Game Object con chứa mô hình (Ví dụ: SM_FantasyFemale)
        animator = GetComponentInChildren<Animator>();

        // Tự động tìm AudioSource trên nhân vật nếu bạn chưa kéo thả vào
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Cần có Object == null vì Update() của Unity chạy liên tục kể cả trước khi máy chủ sinh nhân vật ra
        if (Object == null || !Object.HasInputAuthority) return;

        // Nếu người chơi đang nháy chuột gõ chữ vào khung Chat (hoặc bất kỳ UI nào), thì BỎ QUA mọi phím!
        if (UnityEngine.EventSystems.EventSystem.current != null && 
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null) 
            return;

        // BÀI 1: Bấm chuột trái để tấn công
        if (Input.GetMouseButtonDown(0))
        {
            RpcAttack();
        }

        // BÀI 2: Bấm Space để Nhảy và Phím F để Kỹ Năng
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

        // BÀI 3: Đồng bộ âm thanh tấn công
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RpcJump()
    {
        // Hiển thị thông báo Nhảy theo yêu cầu Bài 2
        Debug.Log($"[Client: {Runner.LocalPlayer}] Nhân vật {Object.Id} đang Nhảy!");
        
        // BÀI 3: Đồng bộ âm thanh nhảy
        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RpcSpecialSkill()
    {
        Debug.Log($"[Client: {Runner.LocalPlayer}] Nhân vật {Object.Id} đang dùng Kỹ năng đặc biệt (Phím F)!");
        
        if (animator != null)
        {
            animator.SetTrigger("Skill");
        }

        // BÀI 3: Đồng bộ âm thanh kỹ năng
        if (audioSource != null && skillSound != null)
        {
            audioSource.PlayOneShot(skillSound);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RpcFootstep()
    {
        // BÀI 3: Đồng bộ tiếng bước chân (Bạn có thể gọi hàm này thông qua Animation Event đi bộ/chạy của Unity)
        if (audioSource != null && footstepSound != null)
        {
            audioSource.PlayOneShot(footstepSound);
        }
    }

    // --- BÀI 3: LOGIC VA CHẠM VÀ MÁU ---
    // Hàm này được gọi từ AnimationEventReceiver khi nắm đấm chạm đích
    public void OnAnimationHit()
    {
        // Vẽ vòng tròn vật lý ảo phía trước nhân vật 1.5m, bán kính 1m
        Vector3 hitCenter = transform.position + transform.forward * 1.5f + Vector3.up * 1f; // Nâng lên 1m để căn giữa thân
        Collider[] hits = Physics.OverlapSphere(hitCenter, 1f);

        foreach (var hit in hits)
        {
            // Nếu trúng chính bản thân mình thì bỏ qua
            if (hit.gameObject == this.gameObject) continue;

            // Nếu trúng nhân vật đứa khác
            PlayerStats targetStats = hit.GetComponent<PlayerStats>();
            if (targetStats != null)
            {
                // SHARED MODE: Ra lệnh chém thẳng vào đứa bị chém (RpcSources.All cho phép đứa chém gọi lệnh lên nạn nhân)
                targetStats.RpcTakeDamage(34, Runner.LocalPlayer);
                break; 
            }
        }
    }
}
