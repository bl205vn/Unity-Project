using UnityEngine;

// ====================================================================
// PlayerController.cs
// Điều khiển Player di chuyển trên địa hình
// - WASD / Arrow Keys để di chuyển
// - Mouse để xoay camera
// - Player va chạm cây to, đi xuyên qua cỏ
// ====================================================================

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("=== MOVEMENT ===")]
    [Tooltip("Tốc độ di chuyển")]
    public float moveSpeed = 8f;
    [Tooltip("Tốc độ chạy (giữ Shift)")]
    public float runSpeed = 14f;
    [Tooltip("Tốc độ nhảy")]
    public float jumpForce = 8f;
    [Tooltip("Trọng lực")]
    public float gravity = -20f;

    [Header("=== CAMERA ===")]
    [Tooltip("Camera theo player (để trống sẽ tự tìm Main Camera)")]
    public Camera playerCamera;
    [Tooltip("Độ nhạy chuột")]
    public float mouseSensitivity = 2f;
    [Tooltip("Góc nhìn lên tối đa")]
    public float maxLookAngle = 80f;
    [Tooltip("Khoảng cách camera phía sau player (0 = FPS)")]
    public float cameraDistance = 5f;
    [Tooltip("Độ cao camera so với player")]
    public float cameraHeight = 2f;

    [Header("=== AUTO SETUP ===")]
    [Tooltip("Tự tạo hình dạng cho Player nếu không có Mesh")]
    public bool autoCreateVisual = true;
    [Tooltip("Màu Player")]
    public Color playerColor = Color.blue;

    // Private
    private CharacterController controller;
    private Vector3 velocity;
    private float xRotation = 0f;
    private bool cursorLocked = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Tự tìm camera
        if (playerCamera == null)
            playerCamera = Camera.main;

        // Tự tạo hình dạng player nếu cần
        if (autoCreateVisual && GetComponentInChildren<MeshRenderer>() == null)
        {
            CreatePlayerVisual();
        }

        // Khóa chuột
        LockCursor(true);

        // Đặt player lên mặt đất
        SnapToGround();
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleCursorLock();
    }

    // ============================================================
    //     DI CHUYỂN
    // ============================================================
    void HandleMovement()
    {
        // Kiểm tra chạm đất
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f; // Giữ player dính mặt đất
        }

        // Input WASD
        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");   // W/S

        // Hướng di chuyển theo hướng player đang nhìn
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Chạy nếu giữ Shift
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Nhảy
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = jumpForce;
        }

        // Áp dụng trọng lực
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // ============================================================
    //     XOAY CAMERA
    // ============================================================
    void HandleMouseLook()
    {
        if (!cursorLocked) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Xoay lên/xuống (giới hạn góc)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        // Xoay player trái/phải
        transform.Rotate(Vector3.up * mouseX);

        // Cập nhật camera
        if (playerCamera != null)
        {
            if (cameraDistance <= 0.1f)
            {
                // FPS Mode: Camera ở đầu player
                playerCamera.transform.position = transform.position + Vector3.up * cameraHeight;
                playerCamera.transform.localRotation = Quaternion.Euler(xRotation, transform.eulerAngles.y, 0f);
            }
            else
            {
                // Third Person Mode: Camera phía sau
                Vector3 targetPos = transform.position + Vector3.up * cameraHeight;
                Vector3 direction = Quaternion.Euler(xRotation, transform.eulerAngles.y, 0f) * Vector3.back;
                
                // Raycast tránh camera xuyên tường
                RaycastHit hit;
                if (Physics.Raycast(targetPos, direction, out hit, cameraDistance))
                {
                    playerCamera.transform.position = hit.point + hit.normal * 0.2f;
                }
                else
                {
                    playerCamera.transform.position = targetPos + direction * cameraDistance;
                }
                
                playerCamera.transform.LookAt(targetPos);
            }
        }
    }

    // ============================================================
    //     CURSOR LOCK
    // ============================================================
    void HandleCursorLock()
    {
        // Nhấn Escape để mở/khóa chuột
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LockCursor(!cursorLocked);
        }

        // Click chuột để khóa lại
        if (!cursorLocked && Input.GetMouseButtonDown(0))
        {
            LockCursor(true);
        }
    }

    void LockCursor(bool locked)
    {
        cursorLocked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    // ============================================================
    //     TỰ TẠO HÌNH DẠNG PLAYER
    // ============================================================
    void CreatePlayerVisual()
    {
        // Tạo thân (Capsule)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "PlayerBody";
        body.transform.SetParent(this.transform);
        body.transform.localPosition = new Vector3(0, 1f, 0);
        body.transform.localScale = new Vector3(0.8f, 1f, 0.8f);

        // Xóa collider của capsule (CharacterController đã có collider)
        Collider bodyCol = body.GetComponent<Collider>();
        if (bodyCol != null) Destroy(bodyCol);

        // Đổi màu
        Renderer bodyRenderer = body.GetComponent<Renderer>();
        if (bodyRenderer != null)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = playerColor;
            bodyRenderer.material = mat;
        }

        // Tạo đầu (Sphere)
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "PlayerHead";
        head.transform.SetParent(this.transform);
        head.transform.localPosition = new Vector3(0, 2.2f, 0);
        head.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        Collider headCol = head.GetComponent<Collider>();
        if (headCol != null) Destroy(headCol);

        Renderer headRenderer = head.GetComponent<Renderer>();
        if (headRenderer != null)
        {
            headRenderer.material = bodyRenderer.material;
        }
    }

    // ============================================================
    //     SNAP TO GROUND
    // ============================================================
    void SnapToGround()
    {
        // Đặt player lên mặt đất tại vị trí hiện tại
        Terrain activeTerrain = Terrain.activeTerrain;
        if (activeTerrain != null)
        {
            Vector3 pos = transform.position;
            float groundY = activeTerrain.SampleHeight(pos) + activeTerrain.transform.position.y;
            transform.position = new Vector3(pos.x, groundY + 1f, pos.z);
        }
    }
}
