using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestTC03
{
    [UnityTest]
    public IEnumerator TestGroundCollision()
    {
        GameObject birdObj = new GameObject("Bird"); birdObj.AddComponent<AudioSource>();
        FlappyBirdScript bird = birdObj.AddComponent<FlappyBirdScript>();
        
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        logicObj.tag = "Logic";
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        logic.G_over = logicObj.AddComponent<AudioSource>();
        logic.gameoverscreen = new GameObject("GameOver");
        
        bird.logic = logic;
        bird.birdisalive = true;

        // Giả lập rớt xuống dưới đáy y < -8
        birdObj.transform.position = new Vector3(0, -9f, 0);

        bird.SendMessage("Update");
        yield return null;

        // Kết quả: Giáp ranh dưới cũng xử thua
        Assert.IsFalse(bird.birdisalive, "Chim rớt xuống dưới biên mặt đất (y < -8) phải bị GameOver.");
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
