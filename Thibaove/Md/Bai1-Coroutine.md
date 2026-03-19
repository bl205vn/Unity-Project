Cách setup trong Unity

- Tạo Plane (mặt đất).
- Tạo Cube (nhân vật).
- Tạo Empty Object đặt tên PointA (vị trí A).

Gắn script vào Cube.

Kéo PointA vào biến Target A trong Inspector.

Nhấn Play và quan sát hoạt động
 
Khi game chạy: Cube đi đến PointA -->Dừng 1 giây --> Sau đó quay vòng quanh trục Y mãi mãi.

```c#
using System.Collections;
using UnityEngine;

public class CoroutineMoveAndRotate : MonoBehaviour
{
    public Transform targetA;   // Vị trí A
    public float moveSpeed = 3f;
    public float rotateSpeed = 120f;

    void Start()
    {
        StartCoroutine(MoveThenRotate());
    }

    IEnumerator MoveThenRotate()
    {
        // --- Di chuyển tới vị trí A ---
        while (Vector3.Distance(transform.position, targetA.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetA.position,
                moveSpeed * Time.deltaTime
            );

            yield return null; // đợi frame tiếp theo
        }

        // --- Chờ 1 giây ---
        yield return new WaitForSeconds(1f);

        // --- Xoay liên tục quanh trục Y ---
        while (true)
        {
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
            yield return null;
        }
    }
    // void OnDrawGizmos()
    // {
    //     if (targetA != null)
    //     {
    //         Gizmos.color = Color.red;
    //         Gizmos.DrawSphere(targetA.position, 0.3f);
    //     }
    // }
}
 ```
