using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class TestLab8_ParallelTesting
{
    private GameObject logicObj;
    private LogicScript logic;

    private GameObject birdObj;
    private FlappyBirdScript bird;

    // ----- SETUP & TEARDOWN -----
    [SetUp]
    public void SetUp()
    {
        // 1. Setup LogicScript
        logicObj = new GameObject("LogicManager");
        logicObj.tag = "Logic";
        logic = logicObj.AddComponent<LogicScript>();

        // Thiết lập UI Text cho Logic
        GameObject textObj = new GameObject("ScoreText");
        Text scoreText = textObj.AddComponent<Text>();
        logic.scoretext = scoreText;

        // Thiết lập Game Over Screen
        GameObject gameOverScreen = new GameObject("GameOverScreen");
        gameOverScreen.SetActive(false); // ẩn mặc định
        logic.gameoverscreen = gameOverScreen;

        // Thiết lập AudioSource cho Game Over
        AudioSource goAudio = logicObj.AddComponent<AudioSource>();
        logic.G_over = goAudio;

        // Khởi tạo Start thủ công (do môi trường Unit Test không tự gọi)
        logic.Start(); 


        // 2. Setup FlappyBirdScript
        birdObj = new GameObject("Bird");
        bird = birdObj.AddComponent<FlappyBirdScript>();
        
        bird.mrg = birdObj.AddComponent<Rigidbody2D>();
        bird.birdfly = birdObj.AddComponent<AudioSource>();
        
        // Cố tình gán reference thay cho việc dùng tag trong test
        bird.logic = logic;
        bird.birdisalive = true;
    }

    [TearDown]
    public void TearDown()
    {
        if (logicObj != null) Object.DestroyImmediate(logicObj);
        if (birdObj != null) Object.DestroyImmediate(birdObj);
        
        // Clean up Text if it's external
        var textObjs = GameObject.FindObjectsOfType<Text>();
        foreach (var t in textObjs) Object.DestroyImmediate(t.gameObject);
    }

// --------------------------------------------------------------------------------------
// LOGIC 1: ĐIỂM SỐ (ADD SCORE) - ÍT NHẤT 3 TEST CASES
// --------------------------------------------------------------------------------------

#if UNITY_EDITOR || UNITY_STANDALONE // Kịch bản kiểm thử trên hệ thống PC
    [Test]
    public void PC_Logic1_Score_BanDau_Bang0()
    {
        Assert.AreEqual(0, logic.score, "[PC] Điểm khởi tạo mặc định phải là 0.");
    }

    [Test]
    public void PC_Logic1_Score_Tang1Diem_KhiGoiHam()
    {
        logic.score = 0;
        logic.addscore();
        Assert.AreEqual(1, logic.score, "[PC] Điểm phải tăng 1 sau khi gọi addscore().");
    }

    [Test]
    public void PC_Logic1_Score_CapNhat_HienThiText()
    {
        logic.score = 5;
        logic.addscore();
        Assert.AreEqual("6", logic.scoretext.text, "[PC] Text phải được update hiển thị đúng điểm số mới thành chữ.");
    }
#endif

#if UNITY_ANDROID || UNITY_IOS // Kịch bản kiểm thử trên hệ thống MOBILE
    [Test]
    public void Mobile_Logic1_Score_BanDau_Bang0()
    {
        Assert.AreEqual(0, logic.score, "[Mobile] Điểm khởi tạo mặc định phải là 0.");
    }

    [Test]
    public void Mobile_Logic1_Score_Tang1Diem_KhiGoiHam()
    {
        logic.score = 0;
        logic.addscore();
        Assert.AreEqual(1, logic.score, "[Mobile] Điểm phải tăng 1 sau khi gọi addscore().");
    }

    [Test]
    public void Mobile_Logic1_Score_CapNhat_HienThiText()
    {
        logic.score = 5;
        logic.addscore();
        Assert.AreEqual("6", logic.scoretext.text, "[Mobile] Text phải được update hiển thị đúng điểm số mới thành chữ.");
    }
#endif

// --------------------------------------------------------------------------------------
// LOGIC 2: GAME OVER (KẾT THÚC GAME) - ÍT NHẤT 3 TEST CASES
// --------------------------------------------------------------------------------------

#if UNITY_EDITOR || UNITY_STANDALONE // Kịch bản kiểm thử trên PC
    [Test]
    public void PC_Logic2_GameOverScreen_HienThi_KhiChet()
    {
        logic.gameover();
        Assert.IsTrue(logic.gameoverscreen.activeSelf, "[PC] Màn hình chết bật khi gọi gameover().");
    }

    [Test]
    public void PC_Logic2_GameOverAudio_DuocPhat()
    {
        logic.gameover();
        Assert.IsTrue(logic.G_over.isPlaying, "[PC] Nhạc Game Over sẽ phát.");
    }

    [Test]
    public void PC_Logic2_GameOverScreen_An_MacDinh()
    {
        Assert.IsFalse(logic.gameoverscreen.activeSelf, "[PC] Màn hình phải ẩn lúc mới bắt đầu trò chơi.");
    }
#endif

#if UNITY_ANDROID || UNITY_IOS // Kịch bản kiểm thử trên MOBILE
    [Test]
    public void Mobile_Logic2_GameOverScreen_HienThi_KhiChet()
    {
        logic.gameover();
        Assert.IsTrue(logic.gameoverscreen.activeSelf, "[Mobile] Màn hình chết bật khi gọi gameover().");
    }

    [Test]
    public void Mobile_Logic2_GameOverAudio_DuocPhat()
    {
        logic.gameover();
        Assert.IsTrue(logic.G_over.isPlaying, "[Mobile] Nhạc Game Over sẽ phát.");
    }

    [Test]
    public void Mobile_Logic2_GameOverScreen_An_MacDinh()
    {
        Assert.IsFalse(logic.gameoverscreen.activeSelf, "[Mobile] Màn hình phải ẩn lúc mới bắt đầu trò chơi.");
    }
#endif

// --------------------------------------------------------------------------------------
// LOGIC 3: TRẠNG THÁI CHIM (BIRD LOGIC) - ÍT NHẤT 3 TEST CASES
// --------------------------------------------------------------------------------------

#if UNITY_EDITOR || UNITY_STANDALONE // Kiểu PC
    [Test]
    public void PC_Logic3_Bird_Alive_MacDinh()
    {
        Assert.IsTrue(bird.birdisalive, "[PC] Chim mới sinh ra phải đang Sống.");
    }

    [Test]
    public void PC_Logic3_Bird_Die_KhiVaCham()
    {
        // Giả lập va chạm với một ống Pipe
        bird.OnCollisionEnter2D(null);
        Assert.IsFalse(bird.birdisalive, "[PC] Chim phải Chết (birdisalive = false) khi va chạm.");
    }

    [Test]
    public void PC_Logic3_Bird_Die_Thì_GoiGameOver()
    {
        bird.OnCollisionEnter2D(null);
        Assert.IsTrue(logic.G_over.isPlaying, "[PC] Chim chết thì hàm gameover() bên Logic kéo theo chạy luôn.");
    }
#endif

#if UNITY_ANDROID || UNITY_IOS // Kiểu Mobile
    [Test]
    public void Mobile_Logic3_Bird_Alive_MacDinh()
    {
        Assert.IsTrue(bird.birdisalive, "[Mobile] Chim mới sinh ra phải đang Sống.");
    }

    [Test]
    public void Mobile_Logic3_Bird_Die_KhiVaCham()
    {
        bird.OnCollisionEnter2D(null);
        Assert.IsFalse(bird.birdisalive, "[Mobile] Chim phải Chết (birdisalive = false) khi va chạm.");
    }

    [Test]
    public void Mobile_Logic3_Bird_Die_Thì_GoiGameOver()
    {
        bird.OnCollisionEnter2D(null);
        Assert.IsTrue(logic.G_over.isPlaying, "[Mobile] Chim chết thì hàm gameover() bên Logic kéo theo chạy luôn.");
    }
#endif

}
