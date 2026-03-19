# Xử lý hành hành vi bằng code

```
using UnityEngine;

public class CubeBehavior : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public Transform player;

    public float speed = 2f;
    public float rotateSpeed = 120f;
    public float detectDistance = 5f;

    private Vector3 target;

    void Start()
    {
        target = pointA.position;
    }

    void Update()
    {
        if (IsPlayerNear())
        {
            Rotate();
        }
        else
        {
            Patrol();
        }
    }

    bool IsPlayerNear()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        return distance < detectDistance;
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
