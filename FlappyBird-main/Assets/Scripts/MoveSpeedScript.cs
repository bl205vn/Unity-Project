/*using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveSpeedScript : MonoBehaviour
{
    public float Movespeed=5;
    public float deadzone = -40;
    public LogicScript logic;
    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + (Vector3.left * Movespeed) * Time.deltaTime;

        if (logic.score % 5 == 0)
        {
            Movespeed += 2;
        }
        if (transform.position.x < deadzone)
        {
            Debug.Log("Pipe Destoyed");
            Destroy(gameObject);
        }

    }
}*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeedScript : MonoBehaviour
{
    public float moveSpeed = 5f; // Changed to camelCase for consistency
    public float deadzone = -40f;
    public LogicScript logic;
    private int lastScore = 0; // Track the last score to detect changes

    void Start()
    {
        GameObject logicObject = GameObject.FindGameObjectWithTag("Logic");
        if (logicObject != null)
        {
            logic = logicObject.GetComponent<LogicScript>();
        }
    }

    void Update()
    {
        // Move the object to the left
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        // Check if the score has changed
        if (logic != null && logic.score != lastScore)
        {
            // If score is divisible by 5 and not the last score we processed
            if (logic.score % 5 == 0 && logic.score != lastScore)
            {
                // Increase speed
                moveSpeed += 2f;
                Debug.Log("Speed Increased: " + moveSpeed);
            }

            // Update the lastScore to the current score
            lastScore = logic.score;
        }

        // Check if the object is past the deadzone
        if (transform.position.x < deadzone)
        {
            Debug.Log("Pipe Destroyed");
            Destroy(gameObject);
        }
    }
}

