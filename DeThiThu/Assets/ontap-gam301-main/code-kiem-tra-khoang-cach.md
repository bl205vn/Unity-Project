```c#
using UnityEngine;

public class CheckDistance : MonoBehaviour
{
    public Transform player;   // kéo Player vào đây trong Inspector

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        Debug.Log("Distance to player: " + distance);
    }
}
```
