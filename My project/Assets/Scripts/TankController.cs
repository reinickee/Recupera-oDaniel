using Photon.Pun;
using UnityEngine;

public class TankController : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 150f;
    public int maxHealth = 100;
    private int currentHealth;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    public GameObject bulletPrefab;
    public Transform firePoint;

    void ClampPosition()
    {
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);

        transform.position = clampedPosition;
    }

    void Awake()
    {
        Camera cam = Camera.main;
        Vector2 screenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.transform.position.z));

        minBounds = new Vector2(-screenBounds.x, -screenBounds.y);
        maxBounds = new Vector2(screenBounds.x, screenBounds.y);
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (!photonView.IsMine)
        {
            Destroy(GetComponent<Rigidbody2D>());
        }
    }

    void Update()
    {
        ClampPosition();
        if (photonView.IsMine)
        {
            HandleMovement();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fire();
            }
        }
    }

    void HandleMovement()
    {
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float rotate = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;

        transform.Translate(Vector3.up * move);
        transform.Rotate(Vector3.forward, -rotate);
    }

    public void Fire()
    {
        if (bulletPrefab && firePoint)
        {
            Bullet bullet = GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.Fire();
            }
        }
    }

    // Removido o método TakeDamage aqui
}
