using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyBirdScript : MonoBehaviour
{
    public Rigidbody2D mrg;
    public float flapstrength;
    public LogicScript logic;
    public bool birdisalive = true;
    public AudioSource birdfly;
    // Start is called before the first frame update
    void Start()
    {
        birdfly=GetComponent<AudioSource>();
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    // Update is called once per frame
    //For Windows,MAC or Linux
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && birdisalive)
        {
            mrg.linearVelocity = Vector2.up * 6;
            birdfly.Play();
        }
        if (transform.position.y > 10 || transform.position.y < -8)
        {
            logic.G_over.Play();
            logic.gameover();
            birdisalive = false;
        }
    }
    //For Android
    /*void Update()
    {
        if (Input.touchCount > 0 && birdisalive)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                mrg.velocity = Vector2.up * 6;
            }
        }

        if (transform.position.y > 10 || transform.position.y < -8)
        {
            logic.gameover();
            birdisalive = false;
        }
    }*/
    public void OnCollisionEnter2D(Collision2D collision)
    {
        logic.G_over.Play();
        logic.gameover();
        birdisalive = false;
    }
}
