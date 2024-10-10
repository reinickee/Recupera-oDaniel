using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviourPun, IDamageable
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 150f;
    public int maxHealth = 100;
    private int currentHealth;
    public Vector2 minBounds;   // Limite mínimo para X e Y
    public Vector2 maxBounds;   // Limite máximo para X e Y


    public GameObject bulletPrefab;
    public Transform firePoint;

    void ClampPosition()
    {
        // Limita a posição do jogador dentro dos bounds
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

        // Apenas o jogador local controla o tanque
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
            PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameManager.instance.UpdateScore(PhotonNetwork.NickName);

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
            Invoke("Respawn", 3f); // Renasce após 3 segundos
        }
    }

    void Respawn()
    {
        Vector3 spawnPosition = GameManager.instance.GetRandomSpawnPosition();
        PhotonNetwork.Instantiate(gameObject.name, spawnPosition, Quaternion.identity);
    }
}