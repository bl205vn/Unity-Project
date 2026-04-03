using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestHV03
{
    [UnityTest]
    public IEnumerator TestPipeDestroyedInDeadZone()
    {
        // 1. Tạo mô hình đường ống
        GameObject pipeObj = new GameObject("Pipe");
        MoveSpeedScript moveScript = pipeObj.AddComponent<MoveSpeedScript>();
        
        // Mô phỏng tọa độ tiệm cận với Deadzone để tiết kiệm thời gian chạy test
        moveScript.deadzone = -40f;
        moveScript.moveSpeed = 100f; // Di chuyển siêu nhanh
        pipeObj.transform.position = new Vector3(-39.9f, 0, 0);

        // Khởi tạo GameManager
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        logicObj.tag = "Logic";
        logicObj.AddComponent<LogicScript>();

        // 2. Gọi ít nhất 2 frame của Unity Engine để pipe thoát khỏi deadzone và destroy
        yield return null; 
        yield return null; 

        // 3. Mất khỏi Engine / Hierachy để bảo vệ RAM
        Assert.IsTrue(pipeObj == null, "Ống đã vượt ranh giới trái nhưng không bị gỡ bỏ. Gây Memory Leak!");
    }
    [SetUp]
    public void SetUp()
    {
        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
        {
            if (go.name != "Main Camera")
            {
                Object.DestroyImmediate(go);
            }
        }
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
