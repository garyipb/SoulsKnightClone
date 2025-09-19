1. Buat Tilemap > Rectangle dengan nama Tilemap childnya Wall
2. Tambahin Tilemap Collider 2D dan Composite Collider 2D
3. Di Tilemap Collider 2D, set Collider Operation menjadi Merge
4. Di Composite Collider 2D, set Geometry Type menjadi Polygons
5. Di Rigidbody 2D, set Body Type menjadi Static
6. Slice tile sprite jadi 16x16, set Pixel Per Unit sama kayak sprite player, set filter mode menjadi Point, dan set Compression menjadi None
7. Drag spritenya ke Tile Palette
8. Bikin tilemap lagi di parent Grid yang sama tanpa collider dengan nama Ground
9. Tambahin Box Collider 2D di player lalu set posisi dan ukurannya
10. Buka Assets/Settings/Renderer2D dan set Transparency Sort Mode menjadi Custom Axis
11. Slice ulang sprite Player biar pivotnya di bawah
12. Buat Sorting Layer baru buat tilemap Ground dengan nama Ground dan posisinya di atas layer Default
13. Tambahin Box Collider 2D di Player biar bisa collide sama dindingnya
14. Buat 2D Object > Sprites > Circle dengan nama Projectile
15. Tambahin Rigidbody 2D dan Circle Collider 2D
16. Tambahin input action baru namanya "Attack" dan bind ke LMB
17. Buat script Projectile.cs
```cs
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Fire(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
```
18.  Edit PlayerController.cs
```cs
public GameObject projectilePrefab;
public Transform firePoint;
public float fireCooldown = 0.2f;
private float fireTimer;
```
```cs
// di Awake()
controls.Player.Shoot.performed += ctx => TryShoot();
```
```cs
// di FixedUpdate()
fireTimer -= Time.fixedDeltaTime;
```
```cs
// tambah method baru
private void TryAttack()
{
    if (fireTimer > 0) return;

    Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    Vector2 shootDir = mouseWorld - transform.position;

    GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    projectile.GetComponent<Projectile>().Fire(shootDir);

    fireTimer = fireCooldown;
}
```
19.  Di Player, tambahin Empty object namanya firePoint
20.  Di component PlayerController, set variabel Fire Point sama Projectile (tinggal drag n drop)
21.  Tambah layer Player dan Projectile dan assign yang sesuai ke Player sama Projectile 
22.  Di Projectile Rigidbody 2D, exclude Player sama Projectile