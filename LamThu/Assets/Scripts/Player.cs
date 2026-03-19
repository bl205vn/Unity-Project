using UnityEngine;

public class Player : MonoBehaviour
{
    public float MoveSpeed = 5f;

    public float rollBoot = 2f;
    private float rollTime;
    public float RollTime;
    bool rollOnce = false;

    private Rigidbody2D rb;
    private Animator animator;

    public SpriteRenderer characterSR;

    public Vector3 MoveInput;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        MoveInput.x = Input.GetAxis("Horizontal");
        MoveInput.y = Input.GetAxis("Vertical");
        transform.position += MoveInput * MoveSpeed * Time.deltaTime;

        animator.SetFloat("Speed", MoveInput.magnitude);

        if (Input.GetKeyDown(KeyCode.Space) && rollTime <=0) 
        {
            animator.SetBool("Roll", true);
            MoveSpeed += rollBoot;
            rollTime = RollTime;
            rollOnce = true;
        }

        if (rollTime <= 0 && rollOnce == true)
        {
            animator.SetBool("Roll", false);
            MoveSpeed -= rollBoot;
            rollOnce = false;
        }
        else
        {
            rollTime -= Time.deltaTime;
        }

        if (MoveInput.x != 0)
        {
            if (MoveInput.x >0)
                characterSR.transform.localScale = new Vector3(1, 1, 1);
            else
                characterSR.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
