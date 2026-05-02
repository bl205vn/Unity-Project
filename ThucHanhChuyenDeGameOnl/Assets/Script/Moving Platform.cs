using UnityEngine;
using Fusion;

public class MovingPlatform : NetworkBehaviour
{
    [Header("Platform Settings")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float delay = 1f;

    // [Networked] để đồng bộ biến giữa các máy
    [Networked] private Vector3 TargetPosition { get; set; }

    // Dùng TickTimer của Fusion thay cho yield return delay của Unity thường
    [Networked] private TickTimer DelayTimer { get; set; }

    public override void Spawned()
    {
        // Khởi tạo vị trí ban đầu (Chỉ máy chủ mới có quyền set)
        if (HasStateAuthority)
        {
            transform.position = pointA.position;
            TargetPosition = pointB.position;
        }
    }

    // Danh sách lưu trữ các Player đang đứng trên bục
    private System.Collections.Generic.List<PlayerMovement> playersOnPlatform = new System.Collections.Generic.List<PlayerMovement>();

    public override void FixedUpdateNetwork()
    {
        // QUAN TRỌNG: Chỉ Server/Host mới tính toán việc đẩy cái bục đi
        // Các Client con chỉ việc nhìn NetworkTransform tự động cập nhật
        if (!HasStateAuthority) return;

        // Nếu đang trong thời gian chờ (delay) thì đứng im
        if (!DelayTimer.ExpiredOrNotRunning(Runner)) return;

        // Ghi nhớ vị trí cũ trước khi di chuyển
        Vector3 previousPosition = transform.position;

        // Dịch chuyển bục từ từ về đích (dùng Runner.DeltaTime thay vì Time.deltaTime)
        transform.position = Vector3.MoveTowards(transform.position, TargetPosition, speed * Runner.DeltaTime);

        // Tính khoảng cách bục vừa dịch chuyển (Delta)
        Vector3 deltaMove = transform.position - previousPosition;

        // Đẩy tất cả player đang đứng trên bục đi theo khoảng cách đó
        foreach (var player in playersOnPlatform)
        {
            if (player != null)
            {
                // Truyền lực sang cho Player tự quyết định ở hàm Move của chính nó
                player.PlatformDeltaMove += deltaMove;
            }
        }

        // Kiểm tra xem đã đến đích chưa
        if (Vector3.Distance(transform.position, TargetPosition) < 0.01f)
        {
            // Đảo mục tiêu
            TargetPosition = TargetPosition == pointA.position ? pointB.position : pointA.position;
            // Reset lại đồng hồ chờ
            DelayTimer = TickTimer.CreateFromSeconds(Runner, delay);
        }
    }

    // --- XỬ LÝ NGƯỜI CHƠI ĐỨNG LÊN BỤC ĐỂ DI CHUYỂN THEO ---
    // Yêu cầu bục phải có 1 Box Collider chỉnh là IsTrigger (nằm hơi nhô lên trên mặt sàn)

    private void OnTriggerEnter(Collider other)
    {
        // Khi Player bước vào, thêm vào danh sách để kéo đi theo
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null && !playersOnPlatform.Contains(pm))
            {
                playersOnPlatform.Add(pm);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Khi Player bước ra, xóa khỏi danh sách
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null && playersOnPlatform.Contains(pm))
            {
                playersOnPlatform.Remove(pm);
            }
        }
    }
}