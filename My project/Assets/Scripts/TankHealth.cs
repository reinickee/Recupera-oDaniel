using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// Classe que gerencia a sa�de do tanque/jogador
public class TankHealth : MonoBehaviourPun, IDamageable
{
    public float maxHealth = 100f; 
    public float currentHealth; 
    public Image healthBarForeground; 
    private bool wasDamagedByOther = false; 

    // Inicializa a sa�de do tanque e atualiza a barra de sa�de
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    // Atualiza a barra de sa�de visualmente
    void UpdateHealthBar()
    {
        if (healthBarForeground != null)
        {
            healthBarForeground.fillAmount = currentHealth / maxHealth; // Atualiza a quantidade preenchida da barra
        }
    }

    // M�todo para receber dano
    public void TakeDamage(float damage, int attackerId)
    {
        if (photonView.ViewID != attackerId) // Apenas recebe dano se o atacante for outro jogador
        {
            photonView.RPC("RPCTakeDamage", RpcTarget.All, damage, attackerId);
        }
    }

    // M�todo RPC que aplica dano e verifica se o tanque morreu
    [PunRPC]
    void RPCTakeDamage(float damageAmount, int attackerId)
    {
        currentHealth -= damageAmount; 
        Debug.Log($"Dano recebido: {damageAmount}, Sa�de atual: {currentHealth}");

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); 
        UpdateHealthBar();

        
        if (attackerId != photonView.ViewID)
        {
            wasDamagedByOther = true;
        }

        if (currentHealth <= 0)
        {
            Die(); 
        }
    }

    // M�todo que trata a morte do tanque
    void Die()
    {
        if (wasDamagedByOther && photonView.IsMine)
        {
            photonView.RPC("OnPlayerDied", RpcTarget.All);

            
            StartCoroutine(Respawn());
        }

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject); // Destr�i o jogador na rede
        }
    }

    // Coroutine para respawn ap�s o tempo definido
    IEnumerator Respawn()
    {
        
        gameObject.SetActive(false);

        
        yield return new WaitForSeconds(3f); 

        
        RespawnManager respawnManager = FindObjectOfType<RespawnManager>();
        if (respawnManager != null)
        {
            respawnManager.RespawnPlayer(gameObject); 
        }
        else
        {
            Debug.LogError("RespawnManager n�o encontrado.");
        }
    }

    // M�todo RPC chamado ap�s a morte do jogador
    [PunRPC]
    public void OnPlayerDied()
    {
        Debug.Log("Um jogador morreu.");
    }
}
