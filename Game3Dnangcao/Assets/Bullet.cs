using UnityEngine;
using UnityEngine.VFX; // <--- BẮT BUỘC PHẢI CÓ DÒNG NÀY (để dùng được chữ VisualEffect)

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float maxLifetime = 5f;
    [SerializeField] private string enemyTag = "Enemy";

    // Thay đổi ở đây: Dùng kiểu VisualEffect thay vì GameObject
    [SerializeField] private VisualEffect explosionEffect; 

    private Vector3 direction;
    private float spawnTime;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Start()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        if (Time.time - spawnTime > maxLifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyTag))
        {
            Debug.Log("Chạm enemy");
            if (explosionEffect != null)
            {
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Debug.Log("Phát nổ");
            }

            Destroy(other.gameObject);
            Debug.Log("Enemy bị tiêu diệt");
            Destroy(gameObject);
        }
    }
}