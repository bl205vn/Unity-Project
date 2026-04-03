using NUnit.Framework;
using UnityEngine;

public class TestTC01
{
    [Test]
    public void TestBirdCollisionDie()
    {
        // 1. Tạo Setup cơ bản
        GameObject birdObj = new GameObject("Bird"); birdObj.AddComponent<AudioSource>();
        FlappyBirdScript bird = birdObj.AddComponent<FlappyBirdScript>();
        
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        bird.logic = logic;
        
        // Bắt buộc xử lý null do Script chứa Audio Source và Screen
        logic.G_over = logicObj.AddComponent<AudioSource>();
        logic.gameoverscreen = new GameObject("GameOverScreen");
        logic.gameoverscreen.SetActive(false); // Trạng thái mặc định là chưa có
        
        bird.birdisalive = true; // Set game starting

        // 2. Chạy cơ chế Collision (Tấn công)
        bird.OnCollisionEnter2D(null); // Force gọi event va chạm (Không cần GameObject thật)

        // 3. Trạng thái sống phải tắt, và Window Failed phải hiện
        Assert.IsFalse(bird.birdisalive, "Sau khi đâm chướng ngại vật chim phải chết.");
        Assert.IsTrue(logic.gameoverscreen.activeSelf, "Màn hình Game Over bắt buộc phải gán On.");
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
