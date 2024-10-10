using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TankController : MonoBehaviourPun, IDamageable, IWeapon
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 150f;
    public int maxHealth = 100;
    private int currentHealth;

    public GameObject bulletPrefab;
    public Transform firePoint;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            HandleMovement();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fire();  // Chama o método Fire da interface IWeapon
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
        PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);
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
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
