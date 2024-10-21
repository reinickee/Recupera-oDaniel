using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public GameObject playerPrefab;
    public Text timerText;
    public Text scoreText;
    public int gameTime = 120; // 2 minutos (120 segundos)

    private bool gameEnded = false;
    private float timeRemaining;
    private Dictionary<string, int> playerScores = new Dictionary<string, int>();

    // M�todo chamado ao iniciar o script
    void Start()
    {
        instance = this;
        timeRemaining = gameTime;

        // Cria um novo jogador na rede
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, GetRandomSpawnPosition(), Quaternion.identity);
        }

        StartCoroutine(GameTimer());
    }

    // M�todo que define o cron�metro
    IEnumerator GameTimer()
    {
        while (timeRemaining > 0 && !gameEnded)
        {
            yield return new WaitForSeconds(1);
            timeRemaining--;
            UpdateTimerUI();
        }

        EndGame();
    }

    // Atualiza a UI do cron�metro
    void UpdateTimerUI()
    {
        timerText.text = "Time: " + Mathf.FloorToInt(timeRemaining).ToString();
    }

    // Atualiza a pontua��o de cada jogador
    public void UpdateScore(string playerName)
    {
        photonView.RPC("RPCUpdateScore", RpcTarget.All, playerName);
    }

    // M�todo RPC para atualizar a pontua��o de um jogador
    [PunRPC]
    void RPCUpdateScore(string playerName)
    {
        if (!playerScores.ContainsKey(playerName))
        {
            playerScores[playerName] = 0;
        }

        playerScores[playerName]++;
        UpdateScoreUI();
    }

    // Atualiza a UI da pontua��o
    void UpdateScoreUI()
    {
        scoreText.text = "";
        foreach (var playerScore in playerScores)
        {
            scoreText.text += playerScore.Key + ": " + playerScore.Value + "\n";
        }
    }

    // M�todo chamado quando o jogo termina
    void EndGame()
    {
        gameEnded = true;
        string winner = GetWinner();
        Debug.Log("Vencedor: " + winner);
        // Aqui voc� pode adicionar l�gica para mostrar o vencedor na UI
    }

    // Determina quem ganhou
    string GetWinner()
    {
        string winner = "";
        int highestScore = 0;

        foreach (var playerScore in playerScores)
        {
            if (playerScore.Value > highestScore)
            {
                highestScore = playerScore.Value;
                winner = playerScore.Key;
            }
        }

        return winner;
    }

    // Retorna uma posi��o aleat�ria no mapa
    public Vector3 GetRandomSpawnPosition()
    {
        // Defina aqui sua l�gica para obter uma posi��o aleat�ria
        float x = Random.Range(-10f, 10f);
        float y = Random.Range(-10f, 10f);
        return new Vector3(x, y, 0);
    }
}