using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BayLuaTrap : MonoBehaviour
{
    [Header("Bẫy lửa")]
    [SerializeField] private float Thoigiantrihoan;
    [SerializeField] private float Thoigiankichhoat;
    private Animator ani;
    private SpriteRenderer sr;

    private bool triggered;
    private bool active;

    private void Awake()
    {
        ani = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "Enemy")
        {
            if (!triggered)
            {
                StartCoroutine(Bayluakichhoat());
            }
            if (active && collision.tag == "Player")
            {
                collision.GetComponent<Player>().Die();
            }
            if (active && collision.tag == "Enemy")
            {
                collision.GetComponent<EnemyMove>().Die();
            }
        }
    }

    private IEnumerator Bayluakichhoat()
    {
        triggered = true;
        sr.color = Color.red;
        yield return new WaitForSeconds(Thoigiantrihoan);
        sr.color = Color.white;
        active = true;
        ani.SetBool("Batlua", true);
        yield return new WaitForSeconds(Thoigiankichhoat);
        active = false;
        triggered = false;
        ani.SetBool("Batlua", false);
    }
} 