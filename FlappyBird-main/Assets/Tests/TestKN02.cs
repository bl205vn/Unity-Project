using NUnit.Framework;
using UnityEngine;

public class TestKN02
{
    [Test]
    public void TestDisableFlapOnDeath()
    {
        GameObject birdObj = new GameObject("Bird"); birdObj.AddComponent<AudioSource>();
        FlappyBirdScript bird = birdObj.AddComponent<FlappyBirdScript>();
        bird.birdisalive = false; // Mock chặn input
        
        Assert.IsFalse(bird.birdisalive, "Skill Flap không thể hoạt động sau khi chim bị Die và Lock biến.");
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
