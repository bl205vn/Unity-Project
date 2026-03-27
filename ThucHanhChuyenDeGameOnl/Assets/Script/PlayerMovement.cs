using UnityEngine;
using Fusion;

public class PlayerMovement : NetworkBehaviour
{
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

        CameraRotation();
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;

        JumpAndGravity();

        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        // Cập nhật hướng xoay của nhân vật dựa theo hướng của Camera (cập nhật trong mạng)
        transform.rotation = Quaternion.Euler(0.0f, _cinemachineTargetYaw, 0.0f);

        // Di chuyển theo hướng nhìn của camera
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        
        Vector3 finalMove = move * speed + new Vector3(0.0f, _verticalVelocity, 0.0f);
        controller.Move(finalMove * Runner.DeltaTime);
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
        if (controller.isGrounded)
        {
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (Input.GetButton("Jump"))
            {
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
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