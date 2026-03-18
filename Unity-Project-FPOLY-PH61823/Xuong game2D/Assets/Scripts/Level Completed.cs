using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleted : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Level Completed!");

            // Nếu là Lv-3 thì chuyển sang scene EndScene
            if (SceneManager.GetActiveScene().name == "Lv-3")
            {
                SceneManager.LoadScene("End");
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
