using UnityEngine;
using Fusion;

public class PlayerMovement : NetworkBehaviour
{
    // Lực đẩy từ Moving Platform truyền sang
    public Vector3 PlatformDeltaMove;

    public CharacterController controller;
    public float speed = 5f;

    [Header("Jump & Gravity")]
    public float jumpHeight = 1.2f;
    public float gravity = -15.0f;
    private float _verticalVelocity;

    [Header("Camera Rotation (StarterAssets style)")]
    public GameObject CinemachineCameraTarget;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraSensitivity = 2.0f; // Thêm tốc độ xoay chuột
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    [Header("Animation Networked States")]
    [Networked] public float AnimSpeed { get; set; }
    [Networked] public float AnimMotionSpeed { get; set; }
    [Networked] public bool AnimGrounded { get; set; }
    [Networked] public bool AnimJump { get; set; }
    [Networked] public bool AnimFreeFall { get; set; }

    public override void Spawned()
    {
        if (CinemachineCameraTarget != null)
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        }

        // Tự động khóa chuột và làm mờ chuột đi giống StarterAssets
        if (Object.HasInputAuthority)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void LateUpdate()
    {
        // Đưa việc xoay Camera ra LateUpdate để cập nhật song song với Frame màn hình (vd 144Hz) thay vì Tick mạng (60Hz) -> Hết giật lag
        if (Object == null || !Object.HasInputAuthority) return;

        // Khóa xoay camera khi đang chat! Nếu không, di chuột tới nút Send sẽ xoay camera lung tung
        if (ChatUI.IsChatting) return;

        CameraRotation();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;

        // Đóng băng di chuyển + nhảy khi đang chat!
        if (ChatUI.IsChatting)
        {
            AnimSpeed = 0f;
            AnimMotionSpeed = 0f;
            return;
        }

        JumpAndGravity();

        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        // Cập nhật hướng xoay của nhân vật dựa theo hướng của Camera (cập nhật trong mạng)
        transform.rotation = Quaternion.Euler(0.0f, _cinemachineTargetYaw, 0.0f);

        // Di chuyển theo hướng nhìn của camera
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        
        // Cập nhật giá trị tốc độ cho Animation lên máy chủ
        AnimSpeed = new Vector2(horizontal, vertical).magnitude * speed;
        AnimMotionSpeed = 1f;

        Vector3 finalMove = move * speed + new Vector3(0.0f, _verticalVelocity, 0.0f);
        // Gộp cả lực di chuyển của Player và lực kéo của Bục vào CÙNG 1 LỆNH MOVE duy nhất
        controller.Move(finalMove * Runner.DeltaTime + PlatformDeltaMove);
        
        // Reset lại lực của bục sau khi đã cộng xong để frame sau tính lại
        PlatformDeltaMove = Vector3.zero; 
    }

    private void CameraRotation()
    {
        // Thay GetAxis bằng GetAxisRaw để bắt trực tiếp tín hiệu chuột, kết hợp nhân với Sensitivity để tăng tốc độ!
        float mouseX = Input.GetAxisRaw("Mouse X") * CameraSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * CameraSensitivity;

        if (Mathf.Abs(mouseX) >= 0.01f || Mathf.Abs(mouseY) >= 0.01f)
        {
            _cinemachineTargetYaw += mouseX;
            _cinemachineTargetPitch -= mouseY;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        if (CinemachineCameraTarget != null)
        {
            // Cinemachine sẽ theo dõi object này
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
        }
    }

    private void JumpAndGravity()
    {
        AnimGrounded = controller.isGrounded;

        if (controller.isGrounded)
        {
            AnimJump = false;
            AnimFreeFall = false;

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (Input.GetButton("Jump"))
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                AnimJump = true; // Kích hoạt nhảy
            }
        }
        else
        {
            if (_verticalVelocity < 0.0f)
            {
                AnimFreeFall = true; // Đang rơi
            }
        }

        _verticalVelocity += gravity * Runner.DeltaTime;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}