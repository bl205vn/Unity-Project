using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogicScript : MonoBehaviour
{
    public int score;
    public Text scoretext;
    public GameObject gameoverscreen;
    public AudioSource G_over;
    [ContextMenu("Increase Score")]
    public void Start()
    {
        G_over= GetComponent<AudioSource>();
    }
    public void addscore()
    {
        score = score + 1;
        scoretext.text = score.ToString();
    }
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void gameover()
    {
        G_over.Play();
        gameoverscreen.SetActive(true);
    }
    public void returnmenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
    }
}
