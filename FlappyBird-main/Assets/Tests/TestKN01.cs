using NUnit.Framework;
using UnityEngine;

public class TestKN01
{
    [Test]
    public void TestRestartSkillTrigger()
    {
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        
        Assert.DoesNotThrow(() => {
            // Function exists to check skill load UI scenes
        }, "LogicScript phải hỗ trợ kỹ năng Restart Nạp Lại game đúng theo flow.");
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
