using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 3;
    public Animator animator;
    public Rigidbody2D rd;
    public float jumpHeight = 5f;
    
    private float movement;
    public float speed = 5f;
    private bool facingRight = true;
    private bool isGrounded = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rd = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (maxHealth <= 0)
        {
            Die();
        }
        movement = Input.GetAxis("Horizontal");
        if (movement < 0 && facingRight)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            facingRight = false;
        }
        else if (movement > 0 && !facingRight)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            facingRight = true;
        }
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            Jump();
            isGrounded = false;
            animator.SetBool("Jump", true);
        }
        if (Mathf.Abs(movement) > 0)
        {
            animator.SetFloat("Run", 1);
        }
        else if (Mathf.Abs(movement) == 0)
        {
            animator.SetFloat("Run", 0);
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            animator.SetTrigger("Attack");
        }
        
    }
    private void FixedUpdate()
    {
        transform.position += new Vector3(movement, 0, 0) * speed * Time.fixedDeltaTime;
    }
    private void Jump()
    {
        rd.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("Jump", false);
        }
    }

    public void TakeDamage(int damage)
    {
        if (maxHealth <= 0)
        {
            return;
        }
        maxHealth -= damage;
    }
    void Die()
    {

    }
}
