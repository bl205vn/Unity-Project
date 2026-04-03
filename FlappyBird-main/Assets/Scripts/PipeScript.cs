using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeScript : MonoBehaviour
{
    public GameObject pipe;
    public float spawnrate = 2;
    public float timer = 0;
    public float height = 10;
    public LogicScript logic;
    // Start is called before the first frame update
    void Start()
    {
        spawnpipe();
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    void Update()
    {
        if (timer < spawnrate)
        {
            timer = timer + Time.deltaTime;
        }
        else
        {
            spawnpipe();
            timer = 0;
        }

    }
    void spawnpipe()
    {
        float lowpoint = transform.position.y - height;
        float highpoint = transform.position.y + height;
        Instantiate(pipe, new Vector3(transform.position.x, Random.Range(lowpoint, highpoint), 0), transform.rotation);
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        logic.gameover();            
    }
}
