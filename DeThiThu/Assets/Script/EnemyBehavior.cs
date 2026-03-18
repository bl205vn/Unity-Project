using System.Collections;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public Transform player; // Kéo thả Player vào đây
    public float attackRange = 10f; // Phạm vi bắn đạn
    public float rotateSpeed = 120f; // Tốc độ xoay
    public GameObject bulletPrefab; // Prefab của viên đạn
    public Transform firePoint; // Vị trí bắn đạn
    public float fireRate = 1f; // Tốc độ bắn (giây/viên)

    private float nextFireTime = 0f;

    void Start()
    {
        // Nếu chưa gán Player trong Inspector thì tự động tìm theo Tag
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Khởi chạy Behavior Tree bằng Coroutine
        StartCoroutine(RunBehaviorTree());
    }

    IEnumerator RunBehaviorTree()
    {
        while (true) // Vòng lặp vô hạn của Behavior Tree
        {
            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (distanceToPlayer <= attackRange)
                {
                    // b) Bắn đạn vào Player nếu nhân vật trong phạm vi 10 đơn vị
                    AttackPlayer();
                }
                else
                {
                    // a) Xoay tròn tại chỗ nếu Player ở ngoài phạm vi
                    RotateIdle();
                }
            }
            else
            {
                // Nếu Player đã bị hủy (hoặc chưa tìm thấy), cứ xoay tròn
                RotateIdle();
            }

            yield return null; // Chờ frame tiếp theo
        }
    }

    void RotateIdle()
    {
        // Quay quanh trục Y
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }

    void AttackPlayer()
    {
        // Quay mặt về phía Player (Chỉ quay trục Y)
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Bỏ qua trục Y để quái không ngửa lên/cúi xuống
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // Bắn đạn nếu đến thời gian chờ
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // Tạo viên đạn tại firePoint
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            
            // Xoay viên đạn hướng thẳng về player
            bullet.transform.LookAt(player.position + Vector3.up * 1f); // Nhắm cao lên 1 tí vào thân Player
        }
        else
        {
            Debug.LogWarning("Chưa gán BulletPrefab hoặc FirePoint cho Enemy!");
        }
    }

    // Vẽ hình tròn đỏ để dễ nhìn thấy phạm vi tấn công trong Scene View
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
