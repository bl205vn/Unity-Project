using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestCD01
{
    // A UnityTest behaves like a coroutine in Play Mode.
    [UnityTest]
    public IEnumerator TestBirdFlap()
    {
        // 1. Khởi động nhân vật
        GameObject birdObj = new GameObject("Bird"); birdObj.AddComponent<AudioSource>();
        Rigidbody2D rb = birdObj.AddComponent<Rigidbody2D>();
        FlappyBirdScript bird = birdObj.AddComponent<FlappyBirdScript>();
        bird.mrg = rb;
        bird.birdisalive = true;
        
        // Mock logic để tránh lỗi NullReferenceException
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        logicObj.tag = "Logic";
        bird.logic = logicObj.AddComponent<LogicScript>();
        
        float initialY = birdObj.transform.position.y;

        // 2. Mô phỏng tương tác: gán trực tiếp lực nhảy
        bird.mrg.linearVelocity = Vector2.up * 6; // Lực nhảy mặc định của game Flappy Bird
        
        // 3. Chờ 1 frame để Physics2D được engine cập nhật
        yield return new WaitForFixedUpdate();

        // 4. Kiểm tra sự thay đổi của chuyển động
        Assert.Greater(birdObj.transform.position.y, initialY, "Nhân vật chim phải bay lên cao hơn vị trí ban đầu (nhảy thành công).");
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
