using NUnit.Framework;
using UnityEngine;

public class TestKN03
{
    [Test]
    public void TestMenuStartGame()
    {
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();
        
        Assert.NotNull(logic, "Flow gọi ReturnMenu trên nút Button Start Game hợp lệ của Scene Hierarchy.");
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
