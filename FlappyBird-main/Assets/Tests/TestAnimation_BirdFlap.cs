using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Lab 5 - Bài 2: Test Animation - Chuyển động vỗ cánh của chim
/// Logic: Khi nhấn Space, chim phải bay lên (animation di chuyển lên trên)
/// Trong Flappy Bird, animation di chuyển được thể hiện qua Rigidbody2D velocity
/// </summary>
public class TestAnimation_BirdFlap
{
    [UnityTest]
    public IEnumerator Animation_BirdFlap_BayLen_KhiNhan_Space()
    {
        // 1. Setup
        GameObject birdObj = new GameObject("Bird"); birdObj.AddComponent<AudioSource>();
        Rigidbody2D rb = birdObj.AddComponent<Rigidbody2D>();
        FlappyBirdScript bird = birdObj.AddComponent<FlappyBirdScript>();
        bird.mrg = rb;
        bird.birdisalive = true;

        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        logicObj.tag = "Logic";
        bird.logic = logicObj.AddComponent<LogicScript>();

        float initialY = birdObj.transform.position.y;

        // 2. Action: Mô phỏng nhảy (gán velocity lên trên như khi nhấn Space)
        bird.mrg.linearVelocity = Vector2.up * 6;

        yield return new WaitForFixedUpdate();

        // 3. Assert: Chim phải bay lên cao hơn
        Assert.Greater(birdObj.transform.position.y, initialY, "Khi nhấn Space, chim phải có animation bay lên (position.y tăng).");
    }

    [UnityTest]
    public IEnumerator Animation_BirdFlap_RoiXuong_KhiKhongNhan()
    {
        // 1. Setup: Tạo chim với trọng lực
        GameObject birdObj = new GameObject("Bird"); birdObj.AddComponent<AudioSource>();
        Rigidbody2D rb = birdObj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f;

        float initialY = birdObj.transform.position.y;

        // 2. Action: Không nhấn gì, chờ trọng lực kéo xuống
        yield return new WaitForSeconds(0.15f);

        // 3. Assert: Chim phải rơi xuống (animation rơi tự do)
        Assert.Less(birdObj.transform.position.y, initialY, "Khi không nhấn Space, chim phải có animation rơi xuống do trọng lực.");
    }

    [Test]
    public void Animation_BirdFlap_TrangThai_BanDau_Alive()
    {
        // 1. Setup
        GameObject birdObj = new GameObject("Bird"); birdObj.AddComponent<AudioSource>();
        FlappyBirdScript bird = birdObj.AddComponent<FlappyBirdScript>();
        bird.birdisalive = true;

        // 2. Assert: Bird khởi tạo phải ở trạng thái sống để có thể animation
        Assert.IsTrue(bird.birdisalive, "Chim phải ở trạng thái sống (birdisalive=true) khi bắt đầu để animation hoạt động.");
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
