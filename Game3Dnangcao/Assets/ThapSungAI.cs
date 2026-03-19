using UnityEngine;

public class ThapSungAI : MonoBehaviour
{
    [Header("Attributes")]
    public float range = 15f; // Vùng bán kính phát hiện enemy
    public float fireRate = 1f;
    public float turnSpeed = 10f;

    [Header("Setup")]
    public string enemyTag = "Enemy";
    [SerializeField] private GameObject bulletPrefab;
    [Tooltip("Vị trí xuất phát đạn (để trống sẽ dùng vị trí tháp)")]
    [SerializeField] private Transform firePoint;

    private Transform target;
    private float fireCountdown = 0f;

    void Start()
    {
        // Cập nhật tìm kiếm mục tiêu mỗi 0.5 giây để tối ưu hiệu năng
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            target = nearestEnemy.transform;
        }
        else
        {
            target = null;
        }
    }

    void Update()
    {
        if (target == null)
            return;

        // Xoay cả tháp súng đến hướng enemy
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        // Chỉ xoay quanh trục Y để tháp súng không bị nghiêng
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        // Bắn
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void Shoot()
    {
        if (target == null || bulletPrefab == null) return;

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        Vector3 dir = (target.position - spawnPos).normalized;

        GameObject bulletObj = Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(dir));
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetDirection(dir);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
