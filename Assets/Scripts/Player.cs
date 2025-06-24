using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Text coinText;
    public int currentCoin = 0;
    public int maxHealth = 100;
    public Text health;
    public Animator animator;
    public Rigidbody2D rd;
    public float jumpHeight = 5f;
    private float movement;
    public float speed = 5f;
    private bool facingRight = true;
    private bool isGrounded = true;
    public Transform attackPoint;
    public float attackRadius = 1f;
    public LayerMask attackLayer;
    public CollisionController collisionController;
    public AudioManager audioManager;

    void Start()
    {
        rd = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();

        collisionController = FindFirstObjectByType<CollisionController>();
        if (collisionController == null)
        {
            Debug.LogError("CollisionController not found in the scene! Game Over screen might not work correctly.");
        }
        audioManager = FindFirstObjectByType<AudioManager>();
        coinText.text = currentCoin.ToString();
        health.text = maxHealth.ToString();
    }

    void Update()
    {
        if (maxHealth <= 0)
        {
            Die();
            return;
        }

        coinText.text = currentCoin.ToString();
        health.text = maxHealth.ToString();

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

        if (Input.GetKeyDown(KeyCode.DownArrow))
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

    public void Attack()
    {
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (collInfo)
        {
            PetrolEnermy enemy = collInfo.gameObject.GetComponent<PetrolEnermy>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }
        }
        if (audioManager != null)
            audioManager.PlaySwordSFX();
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    public void TakeDamage(int damage)
    {
        if (maxHealth <= 0)
        {
            return;
        }
        maxHealth -= damage;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            currentCoin++;
            if (audioManager != null)
            {
                audioManager.PlaySFX();
            }

            if (currentCoin >= 10)
            {
                if (collisionController != null)
                {
                    collisionController.GameWin();
                }
                if (audioManager != null)
                {
                    audioManager.PlayWinSFX();
                }
                this.enabled = false;
            }

            Animator coinAnimator = collision.gameObject.GetComponentInChildren<Animator>();
            if (coinAnimator != null)
            {
                coinAnimator.SetTrigger("Collected");
                Collider2D coinCollider = collision.gameObject.GetComponent<Collider2D>();
                if (coinCollider != null)
                {
                    coinCollider.enabled = false;
                }
                SpriteRenderer coinRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
                if (coinRenderer != null)
                {
                    coinRenderer.enabled = false;
                }
                Destroy(collision.gameObject, 0.5f);
            }
            else
            {
                Destroy(collision.gameObject);
            }
        }
    }

    void Die()
    {
        if (collisionController != null)
        {
            collisionController.GameOver();
        }
        if (audioManager != null)
        {
            audioManager.PlayDieSFX();
        }
        this.enabled = false;
    }
}
