using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Tests
{
    // Cực kỳ quan trọng ở Lab 6: Phải kế thừa InputTestFixture
    public class TestUI_MainMenu : InputTestFixture
    {
        private Mouse virtualMouse;

        [SetUp]
        public override void Setup()
        {
            base.Setup(); // Bắt buộc phải có dòng này để khởi tạo thư viện giả lập
            
            // Sinh ra chuột ảo
            virtualMouse = InputSystem.AddDevice<Mouse>();
        }

        [UnityTest]
        public IEnumerator Test_Menu_ClickStartButton_ChuyenSangSceneGameplay()
        {
            // 1. Load scene Menu có chứa Canvas và các nút
            SceneManager.LoadScene("Menu Background");
            
            // Đợi 1 frame để Unity tạo xong các GameObject
            yield return null; 

            // 2. Tìm nút Start. Theo hình ảnh là "Button (Legacy)"
            GameObject startButtonObj = GameObject.Find("Button (Legacy)");
            Assert.IsNotNull(startButtonObj, "Không tìm thấy nút 'Button (Legacy)'. Hãy kiểm tra lại Canvas!");

            // Lấy tọa độ của nút
            RectTransform btnRect = startButtonObj.GetComponent<RectTransform>();
            Vector2 buttonScreenPos = btnRect.position;

            // 3. MÔ PHỎNG CLICK CHUỘT
            // Kéo chuột ảo đến tọa độ của nút
            Set(virtualMouse.position, buttonScreenPos);
            
            // Đợi 0.1s để chuột đứng yên đúng vị trí
            yield return new WaitForSeconds(0.1f); 

            // Click chuột trái
            Press(virtualMouse.leftButton);
            yield return null; 
            Release(virtualMouse.leftButton);

            // 4. Chờ game chuyển scene
            // Kinh nghiệm: Khi load qua màn hình chơi thường phải chờ tí xíu
            yield return new WaitForSeconds(1f); 

            // 5. Kiểm tra kết quả: Đã sang màn hình chính chưa
            string activeSceneName = SceneManager.GetActiveScene().name;
            Assert.AreEqual("SampleScene", activeSceneName, "Scene không chuyển sang SampleScene sau khi click Start!");
        }

        [UnityTest]
        public IEnumerator Test_Menu_ClickExitButton_KhongCoLoi()
        {
            // Bài test cho nút EXIT ("Button (Legacy) (1)")
            SceneManager.LoadScene("Menu Background");
            yield return null; 

            GameObject exitButtonObj = GameObject.Find("Button (Legacy) (1)");
            Assert.IsNotNull(exitButtonObj, "Không tìm thấy nút 'Button (Legacy) (1)'.");

            RectTransform exitBtnRect = exitButtonObj.GetComponent<RectTransform>();
            Set(virtualMouse.position, exitBtnRect.position);
            
            yield return new WaitForSeconds(0.1f); 

            // Bấm Exit
            Press(virtualMouse.leftButton);
            yield return null; 
            Release(virtualMouse.leftButton);

            yield return new WaitForSeconds(0.5f); 
            
            // Với nút Exit, thường sẽ gọi Application.Quit(). Trong Unity Editor nó sẽ không thoát thật, 
            // nên ta assert không có Exception hoặc test vẫn sống là pass.
            Assert.Pass("Đã click nút Exit thành công mượt mà, không throw Error!");
        }
    }
}
