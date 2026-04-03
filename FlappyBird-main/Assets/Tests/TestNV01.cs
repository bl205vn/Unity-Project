using NUnit.Framework;
using UnityEngine;

public class TestNV01
{
    [Test]
    public void TestScoreIncrease()
    {
        // 1. Setup hệ thống ghi điểm Game (Logic Manager)
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        
        // Mock reference tới canvas Text UI
        GameObject textObj = new GameObject("ScoreText");
        logic.scoretext = textObj.AddComponent<UnityEngine.UI.Text>();
        
        logic.score = 0; // Đặt điểm số mặc định

        // 2. Test hàm nhiệm vụ: Chim lọt qua và ăn điểm (MiddleScript)
        logic.addscore();

        // 3. Thực hiện Assert chứng minh tăng theo mong đợi
        Assert.AreEqual(1, logic.score, "Biến số Score của LogicScript phải tăng 1.");
        Assert.AreEqual("1", logic.scoretext.text, "Bảng Text hiển thị UI phải được render chính xác với điểm.");
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
