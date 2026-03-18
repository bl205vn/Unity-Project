using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerShooter : MonoBehaviour
{
    [Header("Bắn đạn (Shooting)")]
    public GameObject bulletPrefab;
    [Tooltip("Vị trí nòng súng (nơi viên đạn bay ra)")]
    public Transform firePoint;     
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Header("Camera & Ngắm (Aim)")]
    [Tooltip("Kéo Main Camera vào đây")]
    public Camera mainCamera; 
    
    [Tooltip("Layer mà đạn có thể trúng (Nhớ BỎ tick layer Player để đạn không bắn trúng mình)")]
    public LayerMask aimColliderLayerMask = default; 

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        bool isShooting = false;
        
        // Nhận diện GIỮ chuột trái để bắn liên tục
#if ENABLE_INPUT_SYSTEM
        // isPressed thay vì wasPressedThisFrame để kiểm tra nút đang được giữ
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            isShooting = true;
#else
        // GetMouseButton(0) thay vì GetMouseButtonDown(0) để kiểm tra nút đang được giữ
        if (Input.GetMouseButton(0))
            isShooting = true;
#endif

        // Nếu đang giữ nút bắn và đã qua thời gian hồi chiêu
        if (isShooting && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Chưa gắn Bullet Prefab hoặc Fire Point!");
            return;
        }

        // 1. Tính tâm màn hình
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        
        // 2. Bắn tia Raycast từ Camera theo hướng tâm màn hình
        Ray ray = mainCamera.ScreenPointToRay(screenCenterPoint);
        Vector3 aimHitPosition = Vector3.zero;

        // 3. Kiểm tra xem tia quét được vật thể gì không (nhắm vào tường, đất, quái...)
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 999f, aimColliderLayerMask))
        {
            // Điểm chạm thực tế của tâm ngắm
            aimHitPosition = hitInfo.point; 
        }
        else
        {
            // Bắn vào trời/không khí => mục tiêu cực xa
            aimHitPosition = ray.GetPoint(100f); 
        }

        // 4. Sinh viên đạn ở nòng súng
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 5. Chỉnh hướng viên đạn quay mặt về phía mục tiêu nhắm trúng
        Vector3 aimDirection = (aimHitPosition - firePoint.position).normalized;
        bullet.transform.forward = aimDirection;

        // Optional: Chạy âm thanh từ AudioManager bạn vừa tạo
        if (AudioManager.Instance != null && AudioManager.Instance.sfxSource != null)
        {
            // AudioManager.Instance.PlaySFX(tiengBanSung);
        }
    }
}
