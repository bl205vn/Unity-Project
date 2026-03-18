using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private float movespeed = 1f;
    [SerializeField] private float scale = 1f;
    Rigidbody2D myRigidbody;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        myRigidbody.linearVelocity = new Vector2(movespeed, myRigidbody.linearVelocity.y) * scale;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        movespeed= -movespeed;
        FlipEnemyfacing();
        if (other.gameObject.tag == "Bay1" || other.gameObject.tag == "Bay3")
        {
            Die();
        }
    }
    void FlipEnemyfacing()
    {
        transform.localScale = new Vector2(-(Mathf.Sign(myRigidbody.linearVelocity.x)), 1f);
    }

    public void Die() 
    {
        Destroy(gameObject);
    }
}
