using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Lab 5 - Bài 1: Test UI Settings - Các nút điều khiển trên màn hình Game Over
/// Logic: Kiểm tra tồn tại và hoạt động của các chức năng UI (Restart, Return Menu)
/// </summary>
public class TestUI_GameOverButtons
{
    [Test]
    public void UI_RestartButton_HamRestart_TonTai()
    {
        // 1. Setup
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();

        // 2. Assert: Hàm restart() phải tồn tại và không throw exception
        Assert.DoesNotThrow(() =>
        {
            var method = typeof(LogicScript).GetMethod("restart");
            Assert.IsNotNull(method, "Hàm restart() phải tồn tại trong LogicScript.");
        }, "LogicScript phải có chức năng Restart để load lại scene.");
    }

    [Test]
    public void UI_ReturnMenuButton_HamReturnMenu_TonTai()
    {
        // 1. Setup
        GameObject logicObj = new GameObject("LogicManager"); logicObj.AddComponent<AudioSource>();
        LogicScript logic = logicObj.AddComponent<LogicScript>();

        // 2. Assert: Hàm returnmenu() phải tồn tại
        Assert.DoesNotThrow(() =>
        {
            var method = typeof(LogicScript).GetMethod("returnmenu");
            Assert.IsNotNull(method, "Hàm returnmenu() phải tồn tại trong LogicScript.");
        }, "LogicScript phải có nút Return Menu để quay về màn hình chính.");
    }

    [Test]
    public void UI_MenuStartButton_HamStart_TonTai()
    {
        // 1. Setup
        GameObject menuObj = new GameObject("MenuButton");
        MenuButtonScript menu = menuObj.AddComponent<MenuButtonScript>();

        // 2. Assert: Hàm start() trong MenuButtonScript phải tồn tại
        Assert.DoesNotThrow(() =>
        {
            var method = typeof(MenuButtonScript).GetMethod("start");
            Assert.IsNotNull(method, "Hàm start() phải tồn tại trong MenuButtonScript.");
        }, "MenuButtonScript phải có nút Start để bắt đầu game.");
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
