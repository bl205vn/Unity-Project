using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Loadscreen : MonoBehaviour
{
    [Header("Nhập số thứ tự scene (0, 1, 2, 3...)")]
    public int sceneIndex = 0; // 0 = scene đầu tiên, 1 = scene thứ 2, v.v...

    public void LoadManChoi()
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError($"Scene index {sceneIndex} không hợp lệ! (0-{SceneManager.sceneCountInBuildSettings - 1})");
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LoadManChoi();
        }
    }
}
