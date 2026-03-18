using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    [SerializeField] private float moveSpeed = 500f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;

    private string currentAnim;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        float xAxis = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(xAxis) > 0.1f)
        {
           
            rb.linearVelocity = new Vector2(xAxis * moveSpeed * Time.deltaTime, rb.linearVelocity.y);
            transform.rotation = Quaternion.Euler(0, (xAxis > 0) ? 0 : 180, 0);
        }
        else
        {
            
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
           
        }
    }


}