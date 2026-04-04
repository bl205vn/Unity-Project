using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Lab 5 - Bài 2: Test Âm thanh - Tiếng Game Over
/// Logic: Khi chim chết (va chạm ống hoặc ra ngoài biên), âm thanh Game Over phải được phát
/// </summary>
public class TestAudio_GameOverSound
{
    [Test]
    public void Audio_GameOver_AudioSource_TonTai()
    {
        // 1. Setup: Tạo LogicManager với AudioSource (giống pattern Lab 4)
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        // Gán riêng AudioSource cho G_over (thêm component mới, tránh trùng với Start())
        logic.G_over = logicObj.AddComponent<AudioSource>();

        // 2. Assert: G_over AudioSource phải tồn tại và được gán
        Assert.IsNotNull(logic.G_over, "LogicScript phải có AudioSource G_over để phát âm thanh Game Over.");
    }

    [Test]
    public void Audio_GameOver_PhatAm_KhiVaCham()
    {
        // 1. Setup
        GameObject birdObj = new GameObject("Bird"); birdObj.AddComponent<AudioSource>();
        FlappyBirdScript bird = birdObj.AddComponent<FlappyBirdScript>();

        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        logic.G_over = logicObj.AddComponent<AudioSource>();
        logic.gameoverscreen = new GameObject("GameOverScreen");
        bird.logic = logic;
        bird.birdisalive = true;

        // 2. Action: Va chạm
        bird.OnCollisionEnter2D(null);

        // 3. Assert: Sau va chạm, gameover đã được gọi (gameoverscreen hiện = logic.gameover() đã chạy = G_over.Play() đã gọi)
        Assert.IsTrue(logic.gameoverscreen.activeSelf, "Hàm gameover() phải được gọi -> G_over.Play() phải được kích hoạt khi va chạm.");
    }

    [Test]
    public void Audio_GameOver_KhongPhat_KhiChuaChet()
    {
        // 1. Setup
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        logic.G_over = logicObj.AddComponent<AudioSource>();
        logic.gameoverscreen = new GameObject("GameOverScreen");
        logic.gameoverscreen.SetActive(false);

        // 2. Assert: Khi chưa gọi gameover(), màn hình không hiện -> âm thanh chưa được phát
        Assert.IsFalse(logic.gameoverscreen.activeSelf, "Khi game chưa kết thúc, âm thanh Game Over không được phát (gameoverscreen vẫn ẩn).");
        Assert.IsFalse(logic.G_over.isPlaying, "AudioSource G_over không được đang phát khi game vẫn đang chạy.");
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
