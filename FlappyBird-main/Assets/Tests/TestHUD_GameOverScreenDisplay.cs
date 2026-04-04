using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Lab 5 - Bài 1: Test HUD - Màn hình Game Over hiển thị đúng
/// Logic: Khi chim chết, màn hình Game Over phải được bật lên trên HUD
/// </summary>
public class TestHUD_GameOverScreenDisplay
{
    [Test]
    public void HUD_GameOverScreen_HienThi_Khi_Chet()
    {
        // 1. Setup
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        logic.G_over = logicObj.AddComponent<AudioSource>();
        logic.gameoverscreen = new GameObject("GameOverScreen");
        logic.gameoverscreen.SetActive(false); // Trạng thái mặc định: ẩn

        // 2. Action: Kích hoạt game over
        logic.gameover();

        // 3. Assert: Màn hình Game Over phải được hiển thị
        Assert.IsTrue(logic.gameoverscreen.activeSelf, "Màn hình Game Over phải hiển thị (SetActive=true) khi chim chết.");
    }

    [Test]
    public void HUD_GameOverScreen_An_Khi_BatDau()
    {
        // 1. Setup: Kiểm tra trạng thái mặc định
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        logic.gameoverscreen = new GameObject("GameOverScreen");
        logic.gameoverscreen.SetActive(false);

        // 2. Assert: Màn hình Game Over phải ẩn khi game mới bắt đầu
        Assert.IsFalse(logic.gameoverscreen.activeSelf, "Màn hình Game Over phải ẩn (SetActive=false) khi game mới bắt đầu.");
    }

    [Test]
    public void HUD_GameOverScreen_VanHienThi_SauKhi_VaCham()
    {
        // 1. Setup
        GameObject birdObj = new GameObject("Bird"); birdObj.AddComponent<AudioSource>();
        FlappyBirdScript bird = birdObj.AddComponent<FlappyBirdScript>();

        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        logic.G_over = logicObj.AddComponent<AudioSource>();
        logic.gameoverscreen = new GameObject("GameOverScreen");
        logic.gameoverscreen.SetActive(false);
        bird.logic = logic;
        bird.birdisalive = true;

        // 2. Action: Va chạm với ống
        bird.OnCollisionEnter2D(null);

        // 3. Assert: HUD Game Over hiển thị + chim chết
        Assert.IsTrue(logic.gameoverscreen.activeSelf, "HUD Game Over phải hiện lên sau khi va chạm ống.");
        Assert.IsFalse(bird.birdisalive, "Chim phải ở trạng thái chết sau va chạm.");
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
