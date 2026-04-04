using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Lab 5 - Bài 1: Test HUD - Điểm số hiển thị đúng khi thu thập điểm
/// Logic: Khi chim qua ống, điểm số trên HUD phải tăng và hiển thị chính xác
/// </summary>
public class TestHUD_ScoreDisplayOnCollect
{
    [Test]
    public void HUD_DiemSo_HienThi_Dung_Khi_ThuThap_1Diem()
    {
        // 1. Setup: Tạo LogicScript với UI Text hiển thị điểm
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();

        GameObject textObj = new GameObject("ScoreText");
        logic.scoretext = textObj.AddComponent<UnityEngine.UI.Text>();
        logic.score = 0;

        // 2. Action: Chim qua ống - gọi addscore()
        logic.addscore();

        // 3. Assert: HUD phải hiển thị điểm số "1"
        Assert.AreEqual(1, logic.score, "Biến score phải tăng lên 1 sau khi thu thập.");
        Assert.AreEqual("1", logic.scoretext.text, "HUD Text phải hiển thị đúng giá trị '1' trên màn hình.");
    }

    [Test]
    public void HUD_DiemSo_HienThi_Dung_Khi_ThuThap_NhieuDiem()
    {
        // 1. Setup
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();

        GameObject textObj = new GameObject("ScoreText");
        logic.scoretext = textObj.AddComponent<UnityEngine.UI.Text>();
        logic.score = 0;

        // 2. Action: Chim qua 5 ống liên tục
        for (int i = 0; i < 5; i++)
        {
            logic.addscore();
        }

        // 3. Assert: HUD phải hiển thị "5"
        Assert.AreEqual(5, logic.score, "Score phải bằng 5 sau khi thu thập 5 lần.");
        Assert.AreEqual("5", logic.scoretext.text, "HUD Text phải hiển thị '5' chính xác trên giao diện.");
    }

    [Test]
    public void HUD_DiemSo_BatDau_Tu_0()
    {
        // 1. Setup: Kiểm tra trạng thái khởi tạo ban đầu
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();

        // 2. Assert: Điểm số ban đầu phải là 0
        Assert.AreEqual(0, logic.score, "Điểm số ban đầu khi bắt đầu game phải là 0.");
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
