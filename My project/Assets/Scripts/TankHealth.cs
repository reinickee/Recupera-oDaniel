using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TankHealth : MonoBehaviourPun
{
    public float maxHealth = 100f, currentHealth;
    public Image healthBarForeground;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        photonView.RPC("RPCTakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPCTakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"Dano recebido: {damageAmount}, Saúde atual: {currentHealth}");

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarForeground != null)
        {
            healthBarForeground.fillAmount = currentHealth / maxHealth;
        }
    }

    void Die()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject); // Destrói o tanque na rede
        }
    }
}
