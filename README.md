1. Bikin project baru pake template "Universal 2D"
2. Buat folder "Input" dan InputAction baru dengan nama "PlayerControls"
3. Buka InputAction yang baru dan buat action map "Player"
4. Buat action "Move" dan bind WASD
5. Buat action "Dash" dan bind Space
6. Save dan generate C# Class
7. Buat 2D Object > Sprites > Circle dengan nama Player
8. Tambahin component RigidBody 2D ke Player dan set Freeze Rotation Z true dan Gravity Scale 0
9.  Tambahin component baru di Player dengan nama "PlayerController" taro di folder "Scripts"
```cs
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Dash.performed += ctx => TryDash();
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
}
```
11. Buat Player jadi prefab (tinggal drag Player ke project folder) trus taro di folder dengan nama "Prefabs"
12. Buat folder baru "Sprites/Player" buat naro spritenya player
13. Slice setiap sprite player dengan cara buka Sprite Editornya trus pilih slice dan type Grid By Cell Size trus atur cell sizenya jadi 24x24 lalu apply
14. Ubah filter mode dari semua sprite ke Point
15. Drag semua file spritenya ke scene (satu satu)
16. Taro Animation Clip nya ke folder "Animations/Player"
17. Hapus semua Animation Controller yang ga perlu
18. Buat Animation Controller baru dengan nama Player
19. Masukkin semua Animation Clip yang udah kita bikin sebelumnya ke Animation Controller
20. Buat animasi idle jadi default dengan klik kanan lalu Set as Layer Default State
21. Tambahin parameter "IsMoving" dan "MouseY" ke Animation Controller
22. Atur graphnya dan juga buat setiap transisi Has Exit Time = false dan Transition Duration = 0
```cs
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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        controls = new PlayerControls();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Dash.performed += ctx => TryDash();
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
```