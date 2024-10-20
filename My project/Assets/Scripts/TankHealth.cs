using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TankHealth : MonoBehaviourPun, IDamageable
{
    public float maxHealth = 100f, currentHealth;
    public Image healthBarForeground;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }


    void UpdateHealthBar()
    {
        if (healthBarForeground != null)
        {
            healthBarForeground.fillAmount = currentHealth / maxHealth;
        }
    }

    private bool wasDamagedByOther = false; // Flag para controlar se o tanque foi atingido por outro jogador

    public void TakeDamage(float damage, int attackerId)
    {
        if (photonView.ViewID != attackerId) // Apenas recebe dano se o atacante for outro jogador
        {
            photonView.RPC("RPCTakeDamage", RpcTarget.All, damage, attackerId);
        }
    }

    [PunRPC]
    void RPCTakeDamage(float damageAmount, int attackerId)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Dano recebido: {damageAmount}, Saúde atual: {currentHealth}");

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthBar();

        // Define a flag para indicar que o tanque foi atingido por outro jogador
        if (attackerId != photonView.ViewID)
        {
            wasDamagedByOther = true;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Checa se o jogador tomou dano de outro jogador antes de morrer
        if (wasDamagedByOther && photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject); // Destrói o tanque na rede
            Debug.Log("O tanque foi destruído por outro jogador.");
        }
        else
        {
            Debug.Log("O tanque não pode ser destruído por si mesmo.");
        }
    }

}
