## 1. Chuẩn bị trong Unity

Cài Cinemachine trong Package Manager.
Trong Scene tạo:
```
Player (Capsule)
PointX (Empty Object – vị trí X)
Box (Cube ở xa)
Plane
```
## 2. Virtual Camera
```
CM_PlayerCam (theo nhân vật)
CM_BoxCam (nhìn vào hộp)
```
Chú ý: Cam mặc định bị soi chéo xuống, hãy chỉnh lại rotation để soi ngang
### 3. Thiết lập:

#### Camera 1 – theo nhân vật

Follow: Player

Look At: Player

Priority: 10

#### Camera 2 – nhìn hộp

Follow: để trống hoặc Player

Look At: Box

Priority: 0

Lens: 10

### 4. Gắn Script
Script chú ý sử dụng lớp: ```using Unity.Cinemachine;```

Code script:
```c#
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
  
public class CameraBlendExample : MonoBehaviour
{
    public Transform player;
    public Transform targetX;

    public CinemachineCamera playerCam;
    public CinemachineCamera boxCam;

    public float moveSpeed = 3f;

    void Start()
    {
        StartCoroutine(DemoSequence());
    }

    IEnumerator DemoSequence()
    {
        // Di chuyển player tới vị trí X
        while (Vector3.Distance(player.position, targetX.position) > 0.1f)
        {
            player.position = Vector3.MoveTowards(
                player.position,
                targetX.position,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        // Chuyển camera sang Box
        playerCam.Priority = 5;
        boxCam.Priority = 20;

        yield return new WaitForSeconds(2f);

        // Quay lại camera Player
        playerCam.Priority = 20;
        boxCam.Priority = 5;
    }
}
``` 
Gắn script vào một object (ví dụ GameManager) rồi kéo vào các tham số:
```
Player
PointX
CM_PlayerCam
CM_BoxCam
```
