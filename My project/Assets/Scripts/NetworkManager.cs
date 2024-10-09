// Importa bibliotecas necessárias
using UnityEngine; // Para usar funcionalidades do Unity
using Photon.Pun; // Para usar o Photon PUN (Photon Unity Networking)
using Photon.Realtime; // Para usar funcionalidades em tempo real do Photon

// Define a classe NetworkManager que herda de MonoBehaviourPunCallbacks
public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Singleton

    // Declara uma instância estática da classe NetworkManager
    public static NetworkManager instance;

    // Método chamado quando o script é inicializado
    private void Awake()
    {
        // Verifica se a instância é nula
        if (instance == null)
        {
            instance = this; // Define a instância para este objeto
            DontDestroyOnLoad(gameObject); // Não destrói o objeto ao carregar uma nova cena
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Destroi o objeto se já houver uma instância existente
        }
    }

    #endregion

    
    // Método chamado antes do primeiro frame de atualização
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Conecta ao servidor Photon usando configurações definidas
    }
    
    // Método chamado quando conectado ao servidor mestre do Photon
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected Successful"); // Loga uma mensagem no console

        MenuManager.instance.Connected(); // Chama o método Connected do menuManager
    }
    
    // Método para entrar em uma sala com um nome de sala e apelido
    public void JoinRoom(string roomName, string nickname)
    {
        PhotonNetwork.NickName = nickname; // Define o apelido do jogador
        PhotonNetwork.JoinRoom(roomName); // Tenta entrar na sala especificada
    }
    
    // Método para criar uma sala com um nome de sala e apelido
    public void CreateRoom(string roomName, string nickname)
    {
        PhotonNetwork.NickName = nickname; // Define o apelido do jogador
        PhotonNetwork.CreateRoom(roomName); // Cria uma sala com o nome especificado
    }

    // Método para sair da sala atual
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(); // Sai da sala atual
    }
    
    // Método chamado quando um jogador entra na sala
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player " + newPlayer.NickName + " joined room"); // Loga uma mensagem no console
        MenuManager.instance.UpdatePlayerList(GetPlayerList()); // Atualiza a lista de jogadores no menuManager
    }

    // Método chamado quando um jogador sai da sala
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player " + otherPlayer.NickName + " left room"); // Loga uma mensagem no console
        MenuManager.instance.UpdatePlayerList(GetPlayerList()); // Atualiza a lista de jogadores no menuManager
        MenuManager.instance.SetStartButton(PhotonNetwork.IsMasterClient); // Define o botão de iniciar se o jogador for o mestre da sala
    }
    
    // Método chamado quando o jogador entra na sala
    public override void OnJoinedRoom()
    {
        Debug.Log("Player " + PhotonNetwork.NickName + " joined room"); // Loga uma mensagem no console
        MenuManager.instance.UpdatePlayerList(GetPlayerList()); // Atualiza a lista de jogadores no menuManager
        MenuManager.instance.SetStartButton(PhotonNetwork.IsMasterClient); // Define o botão de iniciar se o jogador for o mestre da sala
    }
    
    // Método para carregar uma cena
    public void LoadScene(string sceneName)
    {
        photonView.RPC("LoadSceneRPC", RpcTarget.All, sceneName); // Chama um RPC para carregar a cena em todos os clientes
    }

    // Método RPC para carregar uma cena
    [PunRPC]
    private void LoadSceneRPC(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName); // Carrega a cena especificada
    }

    public GameObject Instantiate(string prefabName, Vector3 position, Quaternion rotation)
    {
        return PhotonNetwork.Instantiate(prefabName, position, rotation);
    }

    // Método para obter a lista de jogadores como string
    public string GetPlayerList()
    {
        string list = ""; // Inicializa uma string vazia

        // Itera sobre a lista de jogadores do Photon
        foreach (var player in PhotonNetwork.PlayerList)
        {
            list += player + "\n"; // Adiciona o jogador à string
        }

        return list; // Retorna a lista de jogadores
    }
}
