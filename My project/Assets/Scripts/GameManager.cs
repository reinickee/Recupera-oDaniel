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

    // Método que define o cronômetro
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

    void UpdateTimerUI()
    {
        timerText.text = "Time: " + Mathf.FloorToInt(timeRemaining).ToString();
    }

    // Atualiza a pontuação de cada jogador
    public void UpdateScore(string playerName)
    {
        if (!playerScores.ContainsKey(playerName))
        {
            playerScores[playerName] = 0;
        }

        playerScores[playerName]++;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        scoreText.text = "";
        foreach (var playerScore in playerScores)
        {
            scoreText.text += playerScore.Key + ": " + playerScore.Value + "\n";
        }
    }

    // Método chamado quando o jogo termina
    void EndGame()
    {
        gameEnded = true;
        string winner = GetWinner();
        Debug.Log("Vencedor: " + winner);
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

    // Retorna uma posição aleatória no mapa
    public Vector3 GetRandomSpawnPosition()
    {
        // Defina aqui sua lógica para obter uma posição aleatória
        float x = Random.Range(-10f, 10f);
        float y = Random.Range(-10f, 10f);
        return new Vector3(x, y, 0);
    }
}
