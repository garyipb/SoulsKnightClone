using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 dashInput;
    private bool isDashing = false;
    private float dashTime;
    private float dashCooldownTime;

    private PlayerControls controls;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireCooldown = 0.2f;
    private float fireTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Dash.performed += ctx => TryDash();

        controls.Player.Attack.performed += ctx => TryAttack();
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable() => controls.Player.Disable();

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashInput.normalized * dashSpeed;
        }
        else
        {
            rb.linearVelocity = moveInput * moveSpeed;
            if (moveInput.sqrMagnitude > 0.01f)
            {
                dashInput = moveInput;
            }
        }

        if (isDashing && Time.time > dashTime)
        {
            isDashing = false;
        }

        UpdateAnimator();

        fireTimer -= Time.fixedDeltaTime;
    }

    private void TryDash()
    {
        if (!isDashing && Time.time > dashCooldownTime && dashInput != Vector2.zero)
        {
            isDashing = true;
            dashTime = Time.time + dashDuration;
            dashCooldownTime = Time.time + dashCooldown;
        }
    }
    private void TryAttack()
    {
        if (fireTimer > 0) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 mouseDir = mouseWorld - transform.position;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().Fire(mouseDir);

        fireTimer = fireCooldown;
    }

    private void UpdateAnimator()
    {
        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 mouseDir = mouseWorld - transform.position;
        animator.SetFloat("MouseY", mouseDir.y);

        spriteRenderer.flipX = mouseDir.x < 0;
    }
}