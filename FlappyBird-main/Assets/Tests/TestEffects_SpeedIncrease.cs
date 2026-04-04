using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Lab 5 - Bài 2: Test Effects - Hiệu ứng tăng tốc ống khi đạt mốc điểm
/// Logic: Mỗi 5 điểm, tốc độ ống tăng thêm 2 (hiệu ứng khó dần)
/// </summary>
public class TestEffects_SpeedIncrease
{
    [Test]
    public void Effects_TocDo_Tang_Khi_Dat_5Diem()
    {
        // 1. Setup
        GameObject pipeObj = new GameObject("Pipe");
        MoveSpeedScript moveScript = pipeObj.AddComponent<MoveSpeedScript>();
        moveScript.moveSpeed = 5f;
        moveScript.deadzone = -40f;

        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        logicObj.tag = "Logic";
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        moveScript.logic = logic;

        // 2. Action: Đặt điểm bằng 5 và gọi Update
        logic.score = 5;
        pipeObj.SendMessage("Update");

        // 3. Assert: Tốc độ phải tăng từ 5 lên 7
        Assert.AreEqual(7f, moveScript.moveSpeed, "Tốc độ ống phải tăng thêm 2 khi đạt mốc 5 điểm (hiệu ứng khó dần).");
    }

    [Test]
    public void Effects_TocDo_KhongTang_Khi_ChuaDat_Moc()
    {
        // 1. Setup
        GameObject pipeObj = new GameObject("Pipe");
        MoveSpeedScript moveScript = pipeObj.AddComponent<MoveSpeedScript>();
        moveScript.moveSpeed = 5f;
        moveScript.deadzone = -40f;

        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        logicObj.tag = "Logic";
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        moveScript.logic = logic;

        // 2. Action: Đặt điểm = 3 (chưa đạt mốc 5)
        logic.score = 3;
        pipeObj.SendMessage("Update");

        // 3. Assert: Tốc độ phải giữ nguyên 5
        Assert.AreEqual(5f, moveScript.moveSpeed, "Tốc độ ống không được tăng khi chưa đạt mốc 5 điểm.");
    }

    [Test]
    public void Effects_TocDo_Tang_Khi_Dat_10Diem()
    {
        // 1. Setup
        GameObject pipeObj = new GameObject("Pipe");
        MoveSpeedScript moveScript = pipeObj.AddComponent<MoveSpeedScript>();
        moveScript.moveSpeed = 5f;
        moveScript.deadzone = -40f;

        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        logicObj.tag = "Logic";
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        moveScript.logic = logic;

        // 2. Action bước 1: Score = 5 -> Update
        logic.score = 5;
        pipeObj.SendMessage("Update");

        // Bước 2: Score = 10 -> Update
        logic.score = 10;
        pipeObj.SendMessage("Update");

        // 3. Assert: Tốc độ phải tăng 2 lần = 5 + 2 + 2 = 9
        Assert.AreEqual(9f, moveScript.moveSpeed, "Tốc độ ống phải tăng 2 lần (mốc 5 và 10) -> 5 + 2 + 2 = 9.");
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
