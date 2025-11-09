using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float chaseRange = 6f;
    public float stopRange = 1f;
    public float detectionDelay = 0.2f;
    public float damage = 25f;
    public float damageCooldown = 0.1f;
    public float damageTimer = 0f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform player;

    private Vector2 moveDir;
    private bool isDead = false;
    private float checkTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void FixedUpdate()
    {
        damageTimer -= Time.fixedDeltaTime;

        if (isDead || player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        checkTimer -= Time.fixedDeltaTime;
        if (checkTimer <= 0f)
        {
            checkTimer = detectionDelay;
            UpdateMovement();
        }

        rb.linearVelocity = moveDir * moveSpeed;
        UpdateAnimator();
    }

    private void UpdateMovement()
    {
        Vector2 dir = (player.position - transform.position);
        float dist = dir.magnitude;

        if (dist <= chaseRange && dist > stopRange)
        {
            moveDir = dir.normalized;
        }
        else moveDir = Vector2.zero;
    }

    private void UpdateAnimator()
    {
        bool isMoving = moveDir.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        animator.SetFloat("DirY", moveDir.y);
        spriteRenderer.flipX = moveDir.x < 0;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        animator.SetBool("IsDead", true);
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.2f);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isDead) return;
        if (damageTimer > 0) return;

        Health health = other.GetComponent<Health>();

        if (other.CompareTag("Player") && health != null)
        {
            health.TakeDamage(damage);
            damageTimer = damageCooldown;
            Debug.Log("damaged");
        }
    }
}