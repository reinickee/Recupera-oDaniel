using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun, IWeapon
{
    public GameObject bulletPrefab; // Prefab da bala (n�o usado nesse script, mas pode ser �til)
    public Transform firePoint; // Ponto de disparo da bala
    public float reloadTime = 1f; // Tempo de recarga
    private float nextFireTime = 0f; // Pr�ximo tempo para disparo
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
            // Instancia a bala na rede
            GameObject bullet = PhotonNetwork.Instantiate("bulletPrefab", firePoint.position, firePoint.rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.SetShooter(photonView.ViewID); // Define quem atirou
            Debug.Log("Atirador ID: " + photonView.ViewID);
        }
    }

    public void SetShooter(int id)
    {
        shooterId = id; // Armazena o ID do jogador que atirou
    }

    [PunRPC]
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto colidido tem o componente TankHealth (um tanque ou jogador)
        TankHealth tankHealth = collision.gameObject.GetComponent<TankHealth>();
        if (tankHealth != null)
        {
            Debug.Log("ID do jogador atingido: " + tankHealth.photonView.ViewID);
            Debug.Log("ID do atirador: " + shooterId);

            // Aplica dano apenas se o jogador atingido n�o for o dono do tiro
            if (tankHealth.photonView.ViewID != shooterId)
            {
                tankHealth.TakeDamage(damageAmount); // Aplica o dano ao jogador atingido
                Debug.Log("Dano aplicado ao jogador com ID: " + tankHealth.photonView.ViewID);
            }
            else
            {
                Debug.Log("O jogador que atirou n�o pode ser atingido.");
            }

            // Desativa a bala em todos os clientes
            photonView.RPC("DisableGameObject", RpcTarget.All);
        }
    }



    [PunRPC]
    void DisableGameObject()
    {
        gameObject.SetActive(false); // Desativa o objeto (a bala) ao colidir
    }
}
