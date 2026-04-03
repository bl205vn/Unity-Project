using NUnit.Framework;
using UnityEngine;

public class TestNV02
{
    [Test]
    public void TestTaskFailOnDeath()
    {
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        
        GameObject textObj = new GameObject("ScoreText");
        logic.scoretext = textObj.AddComponent<UnityEngine.UI.Text>();
        logic.score = 0;

        // Chết trước khi qua ống (Không qua AddScore)
        logic.G_over = logicObj.AddComponent<AudioSource>();
        logic.gameoverscreen = new GameObject("GameOver");
        
        logic.gameover();

        Assert.AreEqual(0, logic.score, "Điểm số không được cộng khi nhiệm vụ vượt ống thất bại.");
        Assert.IsTrue(logic.gameoverscreen.activeSelf);
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
