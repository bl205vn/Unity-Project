using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    // Sự kiện này được kích hoạt tự động bởi các frame bên trong bộ hoạt ảnh của StarterAssets khi nhân vật bước đi
    private void OnFootstep(AnimationEvent animationEvent)
    {
        // Tương lai bạn có thể cho Play âm thanh bước chân (Footstep audio) tại đây nếu muốn!
    }

    // Sự kiện này được kích hoạt khi nhân vật tiếp đất xong
    private void OnLand(AnimationEvent animationEvent)
    {
        // Tương lai có thể cho Play âm thanh "Bịch" khi chạm đất tại đây!
    }
}
