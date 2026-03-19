using UnityEngine;
using StarterAssets; // Bắt buộc dòng này để gọi được ThirdPersonController

public class PlayerBoost : MonoBehaviour
{
    // Biến để lưu tham chiếu đến script điều khiển nhân vật
    private ThirdPersonController _controller;

    // Biến lưu tốc độ gốc
    private float _defaultMoveSpeed;
    private float _defaultSprintSpeed;

    // Cờ kiểm tra xem có đang boost không để tránh bị chồng lặp
    private bool _isBoosting = false;

    void Start()
    {
        // Lấy component ThirdPersonController ngay trên nhân vật
        _controller = GetComponent<ThirdPersonController>();
    }

    // Hàm xử lý khi nhân vật đi vào vùng Trigger của vật thể
    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu vật thể có Tag là "Boost" và nhân vật chưa đang boost
        if (other.gameObject.CompareTag("Boost") && !_isBoosting)
        {
            // Bắt đầu Coroutine đếm ngược và xử lý tăng tốc
            StartCoroutine(ApplySpeedBoost());

            // Tùy chọn: Ẩn hoặc Hủy vật thể Boost đi để trông như đã ăn được
            // Destroy(other.gameObject); // Dùng dòng này nếu muốn xóa vật thể
            other.gameObject.SetActive(false); // Dùng dòng này để ẩn vật thể
        }
    }

    // Coroutine xử lý logic tăng tốc trong 2 giây
    System.Collections.IEnumerator ApplySpeedBoost()
    {
        _isBoosting = true;

        // 1. Lưu lại tốc độ hiện tại (để sau này trả về như cũ)
        _defaultMoveSpeed = _controller.MoveSpeed;
        _defaultSprintSpeed = _controller.SprintSpeed;

        // 2. Tăng gấp đôi tốc độ
        _controller.MoveSpeed *= 2f;
        _controller.SprintSpeed *= 2f;

        // In ra console để kiểm tra (tùy chọn)
        Debug.Log("Tăng tốc! Tốc độ hiện tại: " + _controller.MoveSpeed);

        // 3. Chờ 2 giây
        yield return new WaitForSeconds(2f);

        // 4. Trả về tốc độ ban đầu
        _controller.MoveSpeed = _defaultMoveSpeed;
        _controller.SprintSpeed = _defaultSprintSpeed;

        _isBoosting = false;
        Debug.Log("Hết hiệu ứng Boost.");
    }
}