using UnityEngine;
using UnityEngine.InputSystem; // Bắt buộc để tự bắt phím K

public class SimpleSunEffect : MonoBehaviour
{
    [Header("Tốc độ tua thời gian")]
    public float timeSpeed = 30f; // Tốc độ quay của mặt trời

    [Header("Màu sắc nắng chiều")]
    public Light myLight; // Biến chứa đèn
    public Color afternoonColor = new Color(1f, 0.5f, 0f); // Màu cam
    public Color dayColor = Color.white; // Màu trắng

    void Start()
    {
        // Tự động lấy đèn nếu quên kéo vào
        if (myLight == null) myLight = GetComponent<Light>();
    }

    void Update()
    {
        // --- XỬ LÝ INPUT ĐỘC LẬP ---
        // Kiểm tra bàn phím có tồn tại VÀ phím K đang được giữ
        if (Keyboard.current != null && Keyboard.current.kKey.isPressed)
        {
            // 1. Xoay mặt trời (Tua nhanh thời gian)
            // Xoay quanh trục X (Vector3.right)
            transform.Rotate(Vector3.right * timeSpeed * Time.deltaTime);

            // 2. Hiệu ứng đổi màu nắng chiều
            // Lấy góc quay hiện tại (0-360)
            float angle = transform.rotation.eulerAngles.x;

            // Nếu góc thấp (hoàng hôn hoặc bình minh) thì chuyển màu cam
            if (angle > 150 && angle < 210) // Góc mặt trời lặn
            {
                myLight.color = Color.Lerp(myLight.color, afternoonColor, Time.deltaTime * 5f);
            }
            else
            {
                // Còn lại trả về màu trắng
                myLight.color = Color.Lerp(myLight.color, dayColor, Time.deltaTime * 5f);
            }
        }
    }
}