using NUnit.Framework;
using UnityEngine;

public class TestHV01
{
    [Test]
    public void TestPipeRandomSpawn()
    {
        GameObject spawnerObj = new GameObject("Spawner");
        PipeScript spawner = spawnerObj.AddComponent<PipeScript>();
        
        spawner.pipe = new GameObject("PipePrefab");
        spawner.height = 10f;

        // Trích độ chênh lệch Random height mà AI quái vật (Pipe) sử dụng để sinh ra
        float lowpoint = spawnerObj.transform.position.y - spawner.height;
        float highpoint = spawnerObj.transform.position.y + spawner.height;

        Assert.AreNotEqual(lowpoint, highpoint, "Khoảng cách spawn Pipe phải có sự chênh lệch độ cao ngẫu nhiên.");
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
