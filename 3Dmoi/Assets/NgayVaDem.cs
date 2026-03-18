using UnityEngine;

public class NgayVaDem : MonoBehaviour
{
    [Header("Cài đặt Ánh Sáng")]
    [Tooltip("Kéo object Directional Light từ Hierarchy vào đây")]
    [SerializeField] private Transform matTroiLight; // Biến chứa thông tin vị trí/góc xoay của đèn

    [Header("Cài đặt Thời Gian")]
    [Tooltip("Tốc độ tua nhanh khi bấm phím X. Số càng lớn tua càng nhanh.")]
    [SerializeField] private float tocDoTua = 50f;

    // Update is called once per frame
    void Update()
    {
        // Kiểm tra Input hệ thống cũ (Old Input System)
        // Dùng GetKey để code chạy liên tục khi ĐANG GIỮ phím
        if (Input.GetKey(KeyCode.X))
        {
            ThayDoiThoiGian();
        }
    }

    void ThayDoiThoiGian()
    {
        if (matTroiLight != null)
        {
            // Xoay đèn quanh trục X (trục ngang) để mô phỏng mặt trời mọc/lặn
            // Time.deltaTime giúp chuyển động mượt mà, không phụ thuộc vào FPS của máy
            matTroiLight.Rotate(Vector3.right * tocDoTua * Time.deltaTime);
        }
        else
        {
            Debug.LogWarning("Bạn chưa gán Directional Light vào script!");
        }
    }
}