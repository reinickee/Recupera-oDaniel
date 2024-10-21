using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// Classe que gerencia a saúde do tanque/jogador
public class TankHealth : MonoBehaviourPun, IDamageable
{
    public float maxHealth = 100f; 
    public float currentHealth; 
    public Image healthBarForeground; 
    private bool wasDamagedByOther = false; 

    // Inicializa a saúde do tanque e atualiza a barra de saúde
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    // Atualiza a barra de saúde visualmente
    void UpdateHealthBar()
    {
        if (healthBarForeground != null)
        {
            healthBarForeground.fillAmount = currentHealth / maxHealth; // Atualiza a quantidade preenchida da barra
        }
    }

    // Método para receber dano
    public void TakeDamage(float damage, int attackerId)
    {
        if (photonView.ViewID != attackerId) // Apenas recebe dano se o atacante for outro jogador
        {
            photonView.RPC("RPCTakeDamage", RpcTarget.All, damage, attackerId);
        }
    }

    // Método RPC que aplica dano e verifica se o tanque morreu
    [PunRPC]
    void RPCTakeDamage(float damageAmount, int attackerId)
    {
        currentHealth -= damageAmount; 
        Debug.Log($"Dano recebido: {damageAmount}, Saúde atual: {currentHealth}");

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

    // Método que trata a morte do tanque
    void Die()
    {
        if (wasDamagedByOther && photonView.IsMine)
        {
            photonView.RPC("OnPlayerDied", RpcTarget.All);

            
            StartCoroutine(Respawn());
        }

        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject); // Destrói o jogador na rede
        }
    }

    // Coroutine para respawn após o tempo definido
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
            Debug.LogError("RespawnManager não encontrado.");
        }
    }

    // Método RPC chamado após a morte do jogador
    [PunRPC]
    public void OnPlayerDied()
    {
        Debug.Log("Um jogador morreu.");
    }
}
