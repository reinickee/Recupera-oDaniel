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

    // Método para restringir a posição do tanque dentro dos limites definidos
    void ClampPosition()
    {
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);

        transform.position = clampedPosition; 
    }

    // Método chamado ao iniciar o script
    void Awake()
    {
        Camera cam = Camera.main; 
        Vector2 screenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.transform.position.z)); 

        // Define os limites mínimos e máximos baseados nas dimensões da tela
        minBounds = new Vector2(-screenBounds.x, -screenBounds.y);
        maxBounds = new Vector2(screenBounds.x, screenBounds.y);
    }

    // Método chamado quando o objeto é iniciado
    void Start()
    {
        currentHealth = maxHealth; 

        // Se este não é o objeto controlado pelo jogador local, remove o Rigidbody2D para que o tanque não se mova
        if (!photonView.IsMine)
        {
            Destroy(GetComponent<Rigidbody2D>());
        }
    }

    // Método chamado a cada quadro
    void Update()
    {
        ClampPosition(); // Restringe a posição do tanque
        if (photonView.IsMine) // Verifica se este tanque é controlado pelo jogador local
        {
            HandleMovement(); // Chama o método para tratar o movimento

            // Se a tecla de disparo for pressionada, chama o método Fire
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fire();
            }
        }
    }

    // Método para controlar o movimento e rotação do tanque
    void HandleMovement()
    {
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float rotate = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;
        transform.Translate(Vector3.up * move);
        transform.Rotate(Vector3.forward, -rotate);
    }

    // Método para disparar uma bala
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

}
