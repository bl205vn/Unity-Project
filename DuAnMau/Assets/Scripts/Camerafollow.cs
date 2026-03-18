using UnityEngine;

public class Camerafollow : MonoBehaviour
{
    [SerializeField] private Transform target; // Đối tượng camera sẽ đi theo
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // Khoảng cách so với target

    // Thời gian để camera đuổi kịp target. Càng lớn càng mượt và có độ trễ.
    [SerializeField] private float smoothTime = 0.25f;

    private Vector3 velocity = Vector3.zero; // Biến đệm cho SmoothDamp, không cần chỉnh

    // Hàm Update được gọi mỗi frame
    void LateUpdate() // Dùng LateUpdate cho camera để đảm bảo Player đã di chuyển xong
    {
        if (target == null) return; // Nếu không có target thì không làm gì cả

        // Tính toán vị trí đích
        Vector3 targetPosition = target.position + offset;

        // Di chuyển camera một cách mượt mà
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
