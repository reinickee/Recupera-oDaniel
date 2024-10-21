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

    // M�todo para restringir a posi��o do tanque dentro dos limites definidos
    void ClampPosition()
    {
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minBounds.x, maxBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minBounds.y, maxBounds.y);

        transform.position = clampedPosition; 
    }

    // M�todo chamado ao iniciar o script
    void Awake()
    {
        Camera cam = Camera.main; 
        Vector2 screenBounds = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.transform.position.z)); 

        // Define os limites m�nimos e m�ximos baseados nas dimens�es da tela
        minBounds = new Vector2(-screenBounds.x, -screenBounds.y);
        maxBounds = new Vector2(screenBounds.x, screenBounds.y);
    }

    // M�todo chamado quando o objeto � iniciado
    void Start()
    {
        currentHealth = maxHealth; 

        // Se este n�o � o objeto controlado pelo jogador local, remove o Rigidbody2D para que o tanque n�o se mova
        if (!photonView.IsMine)
        {
            Destroy(GetComponent<Rigidbody2D>());
        }
    }

    // M�todo chamado a cada quadro
    void Update()
    {
        ClampPosition(); // Restringe a posi��o do tanque
        if (photonView.IsMine) // Verifica se este tanque � controlado pelo jogador local
        {
            HandleMovement(); // Chama o m�todo para tratar o movimento

            // Se a tecla de disparo for pressionada, chama o m�todo Fire
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fire();
            }
        }
    }

    // M�todo para controlar o movimento e rota��o do tanque
    void HandleMovement()
    {
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float rotate = Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime;
        transform.Translate(Vector3.up * move);
        transform.Rotate(Vector3.forward, -rotate);
    }

    // M�todo para disparar uma bala
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
