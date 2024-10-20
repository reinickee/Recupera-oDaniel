using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class Bullet : MonoBehaviourPun, IWeapon
{
    public GameObject bulletPrefab; // Prefab da bala (não usado nesse script, mas pode ser útil)
    public Transform firePoint; // Ponto de disparo da bala
    public float reloadTime = 1f; // Tempo de recarga
    private float nextFireTime = 0f; // Próximo tempo para disparo
    public float bulletSpeed = 30f; // Velocidade da bala
    public float damageAmount = 20f; // Quantidade de dano que a bala causa

    private int shooterId; // ID do jogador que atirou

    public float ReloadTime
    {
        get { return reloadTime; }
    }

    void Update()
    {
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }

    public void Fire()
    {
        if (Time.time >= nextFireTime)
        {
            // Instancia a bala na rede usando PhotonNetwork.Instantiate
            GameObject bullet = PhotonNetwork.Instantiate("bulletPrefab", firePoint.position, firePoint.rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();

            // Certifique-se de que o ID do jogador que atirou seja válido
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
                rb.velocity = transform.up * bulletSpeed; // Dispara a bala
            }

            nextFireTime = Time.time + reloadTime; // Define o tempo para o próximo disparo
        }
    }



    public void SetShooter(int id)
    {
        shooterId = id; // Armazena o ID do jogador que atirou
    }

    private bool hasHit = false;

    [PunRPC]
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto colidido tem o componente TankHealth (um tanque ou jogador)
        TankHealth tankHealth = collision.gameObject.GetComponent<TankHealth>();
        if (tankHealth != null && tankHealth.photonView != null && shooterId != 0)
        {
            Debug.Log("ID do jogador atingido: " + tankHealth.photonView.ViewID);
            Debug.Log("ID do atirador: " + shooterId);

            // Aplica dano apenas se o jogador atingido não for o dono do tiro e ambos os IDs são válidos
            if (tankHealth.photonView.ViewID != shooterId)
            {
                tankHealth.TakeDamage(damageAmount, shooterId); // Aplica o dano ao jogador atingido
                Debug.Log("Dano aplicado ao jogador com ID: " + tankHealth.photonView.ViewID);
                // Desativa a bala em todos os clientes
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

    [PunRPC]
    void DisableGameObject()
    {
        gameObject.SetActive(false); // Desativa o objeto (a bala)
    }

}
