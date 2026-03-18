using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathHandler : MonoBehaviour
{
    public string playerTag = "Player";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            Debug.Log("Player died! Transitioning to YouLose scene.");
            SceneManager.LoadSceneAsync("Lose");

        }
        else
        {
            Debug.Log("Collision with: " + collision.gameObject.name);
        }
    }
}
