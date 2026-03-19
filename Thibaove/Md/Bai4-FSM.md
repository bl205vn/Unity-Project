## Bài toán DEMO

- Tạo 1 Cube (đối tượng AI).
- Tạo 2 điểm A và B (Empty Object).
- Có Player (Capsule).
- Khi Player cách < 5 thì Cube dừng và xoay.


## Phần 1: FSM

### Sơ đồ chuyển đổi trạng thái
```
PATROL (đi qua lại A <-> B)
      |
      | playerDistance < 5
      v
ROTATE (đứng yên xoay)
      |
      | playerDistance >= 5
      v
PATROL
```

### Thiết lập trên unity
Tạo các object sau:

- Cube (AI)
- Player (Capsule)
- PointA (Empty)
- PointB (Empty)

Tiếp theo gán script code cho đối tượng Cube (AI)

Sau đó gán giá trị cho các tham số của script

- PointA
- PointB
- Player

Chạy ứng dụng demo với code dưới đây


Code demo:
```c#
using UnityEngine;

public class CubeFSM : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public Transform player;

    public float speed = 2f;
    public float rotateSpeed = 120f;
    public float detectDistance = 5f;

    private Vector3 target;
    private State currentState;

    enum State
    {
        Patrol,
        Rotate
    }

    void Start()
    {
        target = pointA.position;
        currentState = State.Patrol;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                Patrol();

                if (distanceToPlayer < detectDistance)
                {
                    ChangeState(State.Rotate);
                }
                break;

            case State.Rotate:
                Rotate();

                if (distanceToPlayer >= detectDistance)
                {
                    ChangeState(State.Patrol);
                }
                break;
        }
    }

    void ChangeState(State newState)
    {
        currentState = newState;
    }

    void Patrol()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            target = target == pointA.position ? pointB.position : pointA.position;
        }
    }

    void Rotate()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }
}
```
