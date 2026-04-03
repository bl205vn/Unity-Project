using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestHV02
{
    [UnityTest]
    public IEnumerator TestPipeMovementSpeed()
    {
        // 1. Tạo quái (Ống)
        GameObject pipeObj = new GameObject("Pipe");
        MoveSpeedScript moveScript = pipeObj.AddComponent<MoveSpeedScript>();
        
        moveScript.moveSpeed = 10f;
        moveScript.deadzone = -40f;
        
        float initialX = pipeObj.transform.position.x;

        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        logicObj.tag = "Logic";
        logicObj.AddComponent<LogicScript>();

        // 2. Chờ 1 khoản thời gian để ống di chuyển
        yield return new WaitForSeconds(0.1f);

        // 3. Ống đã dời đi
        Assert.Less(pipeObj.transform.position.x, initialX, "Ống phải di chuyển tấn công ổn định từ phải sang trái màn hình.");
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
