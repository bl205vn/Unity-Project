using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Lab 5 - Bài 2: Test Animation - Chuyển động của ống (Pipe)
/// Logic: Ống phải di chuyển từ phải sang trái liên tục (animation di chuyển)
/// </summary>
public class TestAnimation_PipeMovement
{
    [UnityTest]
    public IEnumerator Animation_Pipe_DiChuyen_TuPhaiSangTrai()
    {
        // 1. Setup
        GameObject pipeObj = new GameObject("Pipe");
        MoveSpeedScript moveScript = pipeObj.AddComponent<MoveSpeedScript>();
        moveScript.moveSpeed = 10f;
        moveScript.deadzone = -40f;

        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        logicObj.tag = "Logic";
        logicObj.AddComponent<LogicScript>();

        float initialX = pipeObj.transform.position.x;

        // 2. Action: Chờ ống di chuyển
        yield return new WaitForSeconds(0.1f);

        // 3. Assert: Ống phải di chuyển sang trái (x giảm)
        Assert.Less(pipeObj.transform.position.x, initialX, "Ống phải có animation di chuyển từ phải sang trái (position.x giảm).");
    }

    [UnityTest]
    public IEnumerator Animation_Pipe_TocDo_DiChuyen_DungMacDinh()
    {
        // 1. Setup
        GameObject pipeObj = new GameObject("Pipe");
        MoveSpeedScript moveScript = pipeObj.AddComponent<MoveSpeedScript>();
        moveScript.moveSpeed = 5f; // Tốc độ mặc định
        moveScript.deadzone = -40f;

        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        logicObj.tag = "Logic";
        logicObj.AddComponent<LogicScript>();

        // 2. Assert: Tốc độ ban đầu phải bằng 5
        Assert.AreEqual(5f, moveScript.moveSpeed, "Tốc độ di chuyển mặc định của ống phải là 5 đơn vị/giây.");

        yield return null;
    }

    [UnityTest]
    public IEnumerator Animation_Pipe_BiHuy_KhiRaKhoiManHinh()
    {
        // 1. Setup: Đặt ống gần deadzone
        GameObject pipeObj = new GameObject("Pipe");
        MoveSpeedScript moveScript = pipeObj.AddComponent<MoveSpeedScript>();
        moveScript.deadzone = -40f;
        moveScript.moveSpeed = 200f; // Di chuyển cực nhanh
        pipeObj.transform.position = new Vector3(-39.5f, 0, 0);

        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        logicObj.tag = "Logic";
        logicObj.AddComponent<LogicScript>();

        // 2. Action: Chờ ống vượt deadzone
        yield return null;
        yield return null;

        // 3. Assert: Ống phải bị hủy (Destroy)
        Assert.IsTrue(pipeObj == null, "Ống phải bị hủy khi đi ra khỏi vùng hiển thị (deadzone) để tiết kiệm bộ nhớ.");
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
