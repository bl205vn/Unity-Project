using NUnit.Framework;
using UnityEngine;

public class TestTC02
{
    [Test]
    public void TestPipeEdgeCollision()
    {
        GameObject birdObj = new GameObject("Bird"); birdObj.AddComponent<AudioSource>();
        FlappyBirdScript bird = birdObj.AddComponent<FlappyBirdScript>();
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        bird.logic = logic;
        
        logic.G_over = logicObj.AddComponent<AudioSource>();
        logic.gameoverscreen = new GameObject("GameOverScreen");
        
        bird.birdisalive = true;

        // Mô phỏng quệt quái (OnCollisionEnter2D kích hoạt do BoxCollider)
        bird.OnCollisionEnter2D(null);

        Assert.IsFalse(bird.birdisalive, "Va chạm cạnh mép BoxCollider sinh lý quái vẫn bị tính là đụng ống và chết.");
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
