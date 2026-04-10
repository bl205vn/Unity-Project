using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    // Sự kiện này được kích hoạt tự động bởi các frame bên trong bộ hoạt ảnh của StarterAssets khi nhân vật bước đi
    private void OnFootstep(AnimationEvent animationEvent)
    {
        // Lấy script PlayerActions gắn ở cục tổng bên ngoài (Parent)
        PlayerActions actions = GetComponentInParent<PlayerActions>();
        
        // Cực kỳ quan trọng: Chỉ có máy chủ/máy bấm phím (InputAuthority) mới được quyền phát tín hiệu bước chân của nhân vật lên mạng, 
        // nếu không tất cả các máy sẽ thi nhau gửi tín hiệu rác!
        if (actions != null && actions.HasInputAuthority)
        {
            // SỬA LỖI LẶP ÂM THANH: BlendTree của Unity hay bị lỗi chạy ngầm Animation Walk ngay cả khi đứng yên (Idle).
            // Ta chặn lại bằng cách kiểm tra: Nhân vật phải ĐANG thực sự mấp mé di chuyển thì mới phát tiếng chân!
            CharacterController cc = GetComponentInParent<CharacterController>();
            if (cc != null && cc.velocity.magnitude < 0.1f) return;

            actions.RpcFootstep();
        }
    }

    // Sự kiện này được kích hoạt khi nhân vật tiếp đất xong
    private void OnLand(AnimationEvent animationEvent)
    {
        // Tương lai có thể cho Play âm thanh "Bịch" khi chạm đất tại đây!
    }

    // Nhận sự kiện tát trúng từ Animation
    private void OnHit(AnimationEvent animationEvent)
    {
        PlayerActions actions = GetComponentInParent<PlayerActions>();
        if (actions != null && actions.HasInputAuthority)
        {
            actions.OnAnimationHit();
        }
    }
}
