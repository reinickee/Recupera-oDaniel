using System.Collections;
using Photon.Pun;
using UnityEngine;

// Classe que gerencia o respawn dos jogadores
public class RespawnManager : MonoBehaviourPun
{
    public float respawnTime = 3.0f; 
    public string playerPrefabName = "Tank"; 

    // Fun��o para come�ar o processo de respawn
    public void RespawnPlayer(GameObject player)
    {
        
        TankController tankController = player.GetComponent<TankController>();

        if (tankController != null)
        {
            
            Vector3 respawnPosition = new Vector3(
                Random.Range(tankController.minBounds.x, tankController.maxBounds.x),
                0f, Random.Range(tankController.minBounds.y, tankController.maxBounds.y)
            );

            // Instancia um novo jogador na posi��o de respawn
            PhotonNetwork.Instantiate(playerPrefabName, respawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("TankController n�o encontrado no jogador.");
        }
    }
}
