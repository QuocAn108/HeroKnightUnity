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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rd = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();

        // Sử dụng FindFirstObjectByType để tránh cảnh báo lỗi obsolete
        collisionController = FindFirstObjectByType<CollisionController>();
        if (collisionController == null)
        {
            Debug.LogError("CollisionController not found in the scene! Game Over screen might not work correctly.");
        }

        // Cập nhật UI ban đầu
        coinText.text = currentCoin.ToString();
        health.text = maxHealth.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        // Kiểm tra máu và gọi Die nếu cần
        if (maxHealth <= 0)
        {
            Die();
            return; // Quan trọng: Thoát khỏi Update để ngăn các hành động khác sau khi chết
        }

        // Cập nhật UI mỗi frame
        coinText.text = currentCoin.ToString();
        health.text = maxHealth.ToString();

        // Xử lý di chuyển
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

        // Xử lý nhảy
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            Jump();
            isGrounded = false;
            animator.SetBool("Jump", true);
        }

        // Xử lý animation chạy
        if (Mathf.Abs(movement) > 0)
        {
            animator.SetFloat("Run", 1);
        }
        else if (Mathf.Abs(movement) == 0)
        {
            animator.SetFloat("Run", 0);
        }

        // Xử lý tấn công
        if (Input.GetKeyDown(KeyCode.DownArrow)) // Sử dụng GetKeyDown để chỉ kích hoạt một lần khi nhấn phím
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

    // Xử lý va chạm vật lý (ví dụ: với Ground)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("Jump", false);
        }
    }

    // Hàm tấn công của người chơi (được gọi từ Animation Event)
    public void Attack()
    {
        Collider2D collInfo = Physics2D.OverlapCircle(attackPoint.position, attackRadius, attackLayer);
        if (collInfo)
        {
            PetrolEnermy enemy = collInfo.gameObject.GetComponent<PetrolEnermy>(); // Lấy component PetrolEnermy

            if (enemy != null)
            {
                enemy.TakeDamage(1);
            }
          
        }
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

    // Hàm nhận sát thương của người chơi
    public void TakeDamage(int damage)
    {
        if (maxHealth <= 0)
        {
            return;
        }
        maxHealth -= damage;
    }

    // Xử lý va chạm Trigger (ví dụ: với Coin)
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            currentCoin++;

            // Tìm Animator trên đối tượng con của đồng xu
            Animator coinAnimator = collision.gameObject.GetComponentInChildren<Animator>();

            if (coinAnimator != null)
            {
                coinAnimator.SetTrigger("Collected");

                // Vô hiệu hóa Collider của đồng xu để không nhặt lại được
                Collider2D coinCollider = collision.gameObject.GetComponent<Collider2D>();
                if (coinCollider != null)
                {
                    coinCollider.enabled = false;
                }
                // Vô hiệu hóa Sprite Renderer của đồng xu (tùy chọn) để nó biến mất ngay lập tức hoặc sau animation
                SpriteRenderer coinRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
                if (coinRenderer != null)
                {
                    coinRenderer.enabled = false;
                }

                Destroy(collision.gameObject, 0.5f); // 0.5 giây là thời gian giả định cho animation chạy hết
            }
            else
            {
                Destroy(collision.gameObject); // Hủy ngay lập tức nếu không có animation
            }
        }
    }

    // Hàm chết của người chơi
    void Die()
    {
        if (collisionController != null)
        {
            collisionController.GameOver(); 
        }
        // Vô hiệu hóa script điều khiển người chơi
        this.enabled = false;
    }
}