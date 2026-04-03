using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestCD03
{
    [UnityTest]
    public IEnumerator TestTopBoundary()
    {
        GameObject birdObj = new GameObject("Bird"); birdObj.AddComponent<AudioSource>();
        FlappyBirdScript bird = birdObj.AddComponent<FlappyBirdScript>();
        
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        logicObj.tag = "Logic";
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        logic.G_over = logicObj.AddComponent<AudioSource>();
        logic.gameoverscreen = new GameObject("GameOver");
        logic.gameoverscreen.SetActive(false);
        bird.logic = logic;
        
        bird.birdisalive = true;
        
        // Mô phỏng chim bay vượt biên trên (Tọa độ y > 10 dựa theo FlappyBirdScript)
        birdObj.transform.position = new Vector3(0, 11f, 0);

        bird.SendMessage("Update"); // Thực thi 1 frame Update logic
        yield return null;

        // Kết quả: Chim chết (birdIsAlive = false)
        Assert.IsFalse(bird.birdisalive, "Chim bay vượt biên trên bề mặt màn hình nên phải bị trigger chết.");
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
