using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Gọi khi nhấn nút Play
    public void PlayGame()
    {
        SceneManager.LoadScene("Lv-1");
    }

    // Gọi khi nhấn nút How To Play
    public void HowToPlay()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    // Gọi khi nhấn nút Exit
    public void ExitGame()
    {
        Debug.Log("Thoát game");
        Application.Quit();

        // Nếu đang test trong Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Gọi khi nhấn nút Back (ở trong HowToPlay)
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
