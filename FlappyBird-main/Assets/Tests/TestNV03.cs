using NUnit.Framework;
using UnityEngine;

public class TestNV03
{
    [Test]
    public void TestPipeSpeedIncreasesEvery5Score()
    {
        // Khởi tạo các Script
        GameObject pipeObj = new GameObject("Pipe");
        MoveSpeedScript moveScript = pipeObj.AddComponent<MoveSpeedScript>();
        moveScript.moveSpeed = 5f; 
        moveScript.deadzone = -40f;
        
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        logicObj.tag = "Logic";
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        moveScript.logic = logic;

        // Hành động 1: Cung cấp Milestone score là 5 vào Logic Manager
        logic.score = 5;

        // Mô tả hành động 2: Gọi Update ở Game Loop để di chuyển vận tốc
        pipeObj.SendMessage("Update");

        // Action 3: Check Event kết quả tính toán có tăng tốc hay chưa
        Assert.AreEqual(7f, moveScript.moveSpeed, "Tốc độ phải cộng thêm 2 sau mỗi 5 điểm (Nhiệm vụ vượt mốc).");
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
