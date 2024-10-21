using UnityEngine;
using Photon.Pun;

// Classe que representa a bala disparada
[RequireComponent(typeof(PhotonView))]
public class Bullet : MonoBehaviourPun, IWeapon
{
    public GameObject bulletPrefab; 
    public Transform firePoint;
    public float reloadTime = 1f; 
    private float nextFireTime = 0f; 
    public float bulletSpeed = 30f;
    public float damageAmount = 20f; 

    private int shooterId; 

    // Propriedade para obter o tempo de recarga
    public float ReloadTime
    {
        get { return reloadTime; }
    }

    // Atualiza a cada frame, verifica se o jogador pode disparar
    void Update()
    {
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Space))
        {
            Fire(); 
        }
    }

    // Método para disparar a bala
    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            
            GameObject bullet = PhotonNetwork.Instantiate("bulletPrefab", firePoint.position, firePoint.rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();

            
            if (photonView != null)
            {
                bulletScript.SetShooter(photonView.ViewID); // Define o ID de quem atirou
                Debug.Log("Atirador ID (photonView): " + photonView.ViewID);
            }
            else
            {
                Debug.LogError("PhotonView não encontrado no jogador!");
            }

            // Aplica movimento à bala
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = transform.up * bulletSpeed; 
            }

            nextFireTime = Time.time + reloadTime; 
        }
    }

    // Método para definir o ID do jogador que atirou
    public void SetShooter(int id)
    {
        shooterId = id; 
    }

    private bool hasHit = false; // Flag para controlar se a bala já atingiu um alvo

    [PunRPC]
    void OnTriggerEnter2D(Collider2D collision)
    {
        
        TankHealth tankHealth = collision.gameObject.GetComponent<TankHealth>();
        if (tankHealth != null && tankHealth.photonView != null && shooterId != 0)
        {
            Debug.Log("ID do jogador atingido: " + tankHealth.photonView.ViewID);
            Debug.Log("ID do atirador: " + shooterId);

            
            if (tankHealth.photonView.ViewID != shooterId)
            {
                tankHealth.TakeDamage(damageAmount, shooterId); 
                Debug.Log("Dano aplicado ao jogador com ID: " + tankHealth.photonView.ViewID);
                
                photonView.RPC("DisableGameObject", RpcTarget.All);
            }
            else
            {
                Debug.Log("O jogador que atirou não pode ser atingido.");
            }
        }
        else
        {
            Debug.LogWarning("O ID do jogador ou atirador é inválido.");
        }
    }

    // Método chamado para desativar o objeto (a bala)
    [PunRPC]
    void DisableGameObject()
    {
        gameObject.SetActive(false); // Desativa o objeto
    }
}
