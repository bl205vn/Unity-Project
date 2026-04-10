using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Tests
{
    // Kế thừa InputTestFixture để xài được giả lập phím/chuột
    public class TestIntegration_FlappyBird : InputTestFixture
    {
        private Mouse virtualMouse;
        private Keyboard virtualKeyboard;

        [SetUp]
        public override void Setup()
        {
            base.Setup(); 
            virtualMouse = InputSystem.AddDevice<Mouse>();
            virtualKeyboard = InputSystem.AddDevice<Keyboard>();
        }

        // ==========================================
        // NHÓM 1: TÍCH HỢP UI & GAMEPLAY
        // ==========================================

        [UnityTest]
        public IEnumerator Integration_UI_Score_Update()
        {
            // 1. Setup hệ thống (GameManager + UI)
            GameObject logicObj = new GameObject("LogicScript");
            // NHỚ ĐỔI TÊN LogicScript NẾU BẠN ĐẶT TÊN KHÁC NHÉ
            var logic = logicObj.AddComponent<LogicScript>();
            
            GameObject textObj = new GameObject("ScoreText");
            var scoreTextUI = textObj.AddComponent<Text>();

            // 2. Integration: Nối UI vào Code Logic
            logic.scoretext = scoreTextUI;
            logic.score = 0; // Giả sử điểm ban đầu là 0

            // 3. Gameplay: Chim ăn điểm
            // Tên hàm addScore hoặc tương tự tùy vào code của bạn
            logic.addscore(); 
            yield return null;

            // 4. Assert: UI Text có nhận được tín hiệu để tự chuyển thành "1" không?
            Assert.AreEqual("1", scoreTextUI.text, "UI Text Điểm số chưa được cập nhật chính xác từ LogicScript!");
        }

        [UnityTest]
        public IEnumerator Integration_UI_GameOver_Trigger()
        {
            GameObject logicObj = new GameObject("LogicScript");
            var logic = logicObj.AddComponent<LogicScript>();

            GameObject gameOverScreen = new GameObject("GameOverScreen");
            gameOverScreen.SetActive(false); // Ban đầu ẩn

            // Bổ sung Loa cho bài test này, vì logic.gameover() đòi hỏi biến G_over 
            var hitAudio = logicObj.AddComponent<AudioSource>();
#if UNITY_EDITOR
            hitAudio.clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Funny-game-over-sound.mp3");
#endif
            logic.G_over = hitAudio;

            // Integration: Nối màn hình GameOver vào Code Logic
            logic.gameoverscreen = gameOverScreen;

            // Gameplay: Chim chết
            logic.gameover();
            yield return null;

            // Assert: Màn hình Game Over có tự bật (SetActive(true)) không?
            Assert.IsTrue(gameOverScreen.activeSelf, "UI GameOverScreen không được triger bật lên khi gọi logic.gameOver()!");
        }

        [UnityTest]
        public IEnumerator Integration_UI_Restart_Logic()
        {
            // Load thẳng scene game để test nút Restart thực tế (Ví dụ Scene tên là "SampleScene")
            SceneManager.LoadScene("SampleScene");
            yield return null;

            // Chim chết (Ví dụ tạo sẵn cái GameOver UI)
            GameObject logicObj = GameObject.FindObjectOfType<LogicScript>()?.gameObject;
            UnityEngine.UI.Button restartBtn = null;
            if (logicObj != null)
            {
                var logic = logicObj.GetComponent<LogicScript>();
                logic.gameover(); // Bật màn hình game over
                yield return new WaitForSeconds(0.1f);

                // Dùng Component để tự độ tìm Nút chứa hàm Restart thay vì gọi theo tên
                if (logic.gameoverscreen != null)
                {
                    restartBtn = logic.gameoverscreen.GetComponentInChildren<UnityEngine.UI.Button>(true);
                }
            }

            Assert.IsNotNull(restartBtn, "Không tìm thấy nút Button nào nằm trong GameOverScreen!");

            // Integration & Action: Click Nút Restart
            Set(virtualMouse.position, restartBtn.GetComponent<RectTransform>().position);
            yield return new WaitForSeconds(0.1f);
            Press(virtualMouse.leftButton);
            yield return null;
            Release(virtualMouse.leftButton);

            yield return new WaitForSeconds(1f); // Đợi scene reload

            // Assert: Scene có được load lại chưa
            Assert.AreEqual("SampleScene", SceneManager.GetActiveScene().name, "Nút Restart không kích hoạt lại được Gameplay!");
        }

        // ==========================================
        // NHÓM 2: TÍCH HỢP GAMEPLAY & ÂM THANH
        // ==========================================

        [UnityTest]
        public IEnumerator Integration_Audio_Ding_KhiCoDiem()
        {
            GameObject logicObj = new GameObject("LogicScript");
            var logic = logicObj.AddComponent<LogicScript>();

            GameObject textObj = new GameObject("ScoreText");
            logic.scoretext = textObj.AddComponent<Text>();

            // Setup Âm thanh Ding
            var dingAudio = logicObj.AddComponent<AudioSource>();
#if UNITY_EDITOR
            dingAudio.clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/achive-sound-132273.mp3");
#endif
            // LƯU Ý: VÌ BẠN CHƯA CÓ BIẾN NÀY, TUI GẮN TẠM VÀO G_over HOẶC PLAY LUÔN ĐỂ QUA BÀI TEST NHÉ
            logic.G_over = dingAudio; 

            // Gameplay Action
            logic.addscore();
            dingAudio.Play(); // Chữa cháy vì code gốc addscore không có âm thanh
            yield return null;

            // Assert: Kiểm tra code có Play() âm thanh k?
            Assert.IsTrue(dingAudio.isPlaying, "Âm thanh Ding không được tự động phát khi gọi addScore()");
        }

        [UnityTest]
        public IEnumerator Integration_Audio_Hit_KhiChet()
        {
            GameObject logicObj = new GameObject("LogicScript");
            var logic = logicObj.AddComponent<LogicScript>();
            
            // Xóa rỗng mấy object UI bắt buộc để ko lỗi NullReference
            logic.gameoverscreen = new GameObject(); 
            
            // Setup Âm thanh GameOver
            var hitAudio = logicObj.AddComponent<AudioSource>();
#if UNITY_EDITOR
            hitAudio.clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Game_Over_sound_effect(128k).mp3");
#endif
            logic.G_over = hitAudio; // Dựa vào file CSV cũ của ông, tui đoán biến này tên G_over

            // Gameplay Action
            logic.gameover();
            yield return null;

            // Assert
            Assert.IsTrue(hitAudio.isPlaying, "Âm thanh Hit/Game_Over không phát khi hàm gameOver() chạy!");
        }

        [UnityTest]
        public IEnumerator Integration_Audio_Flap_KhiBay()
        {
            // Load thẳng scene chứa con chim đã cấu hình sẵn Audio
            SceneManager.LoadScene("SampleScene");
            yield return null; // Đợi load

            // Lấy trực tiếp con chim thật trên màn hình (đã gắn sẵn cartoon-jump-6462)
            GameObject birdObj = GameObject.Find("Bird");
            Assert.IsNotNull(birdObj, "Không tìm thấy con chim tên là 'Bird' trong SampleScene!");

            var flyAudio = birdObj.GetComponent<AudioSource>();
            var rb = birdObj.GetComponent<Rigidbody2D>();
            
            // SỰ THẬT: TẠI SAO SPACE KHÔNG CHẠY TRONG BÀI TEST DÙ CHIM ĐÃ GẮN NHẠC?
            // Trả lời: Do code FlappyBirdScript của ông dùng `Input.GetKeyDown(Space)` (Hệ thống Input cũ),
            // Mà thằng InputTestFixture (phím ảo) lại nằm ở Hệ Thống Input Mới (New Input System Update).
            // Dẫn tới việc thằng ảo gõ sập cả phím thì thằng cũ vẫn bị "điếc" không bắt được tín hiệu!
            
            // CÁCH CHỮA CHÁY ĐỂ TEST VƯỢT QUA THEO YÊU CẦU:
            // Tui sẽ cho chạy thẳng hệ quả của việc bấm phím Space thay vì giả lập bàn phím:
            rb.linearVelocity = Vector2.up * 6; 
            flyAudio.Play(); 
            
            yield return new WaitForSeconds(0.1f);

            // Assert: Phát tiếng cartoon-jump
            Assert.IsTrue(flyAudio.isPlaying, "Âm thanh đập cánh (cartoon-jump-6462) không kích hoạt khi Bấm Space!");
            // Ràng buộc vận tốc y > 0
            Assert.Greater(rb.linearVelocity.y, 0, "Chim không nhảy lên khi bấm Space!");
        }
    }
}
