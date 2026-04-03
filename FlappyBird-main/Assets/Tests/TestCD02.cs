using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestCD02
{
    [UnityTest]
    public IEnumerator TestGravityPull()
    {
        // 1. Gọi và gắn Rigidbody2D cho object ảo
        GameObject birdObj = new GameObject("Bird"); birdObj.AddComponent<AudioSource>();
        Rigidbody2D rb = birdObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f; // Đảm bảo trọng lực được sử dụng
        
        float initialY = birdObj.transform.position.y;

        // 2. Mô phỏng thả rơi tự do: Không tương tác và chờ vật lý
        yield return new WaitForSeconds(0.1f);

        // 3. Kiểm chứng
        Assert.Less(birdObj.transform.position.y, initialY, "Nhân vật tự động rớt xuống do tác động của trọng lực môi trường.");
    }

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
        {
            if (go.name != "Main Camera")
            {
                Object.DestroyImmediate(go);
            }
        }
    }
}
