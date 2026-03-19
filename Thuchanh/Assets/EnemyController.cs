using UnityEngine;
using UnityEngine.AI; // Bắt buộc phải có dòng này để dùng NavMesh

public class EnemyController : MonoBehaviour
{
    public Transform player;      // Biến để kéo nhân vật Player vào
    private NavMeshAgent agent;   // Biến tham chiếu đến NavMeshAgent

    void Start()
    {
        // Tự động lấy component NavMeshAgent gắn trên Enemy
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // 1. Tính khoảng cách giữa Enemy và Player
        float distance = Vector3.Distance(transform.position, player.position);

        // 2. Kiểm tra điều kiện đề bài
        // Nếu khoảng cách nhỏ hơn 8f VÀ lớn hơn 1.5f (chưa quá gần)
        if (distance < 8f && distance > 1.5f)
        {
            agent.isStopped = false; // Cho phép di chuyển
            agent.SetDestination(player.position); // Đặt đích đến là vị trí Player
        }
        else
        {
            // Nếu xa quá (> 8f) hoặc gần quá (< 1.5f) thì dừng lại
            agent.isStopped = true;
        }
    }
}