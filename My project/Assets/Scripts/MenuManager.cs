// Importa as bibliotecas necessárias para collections, Unity Engine e UI.
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Define a classe MenuManager que herda de MonoBehaviour, permitindo que seja usada como um componente em GameObjects.
public class MenuManager : MonoBehaviour
{
    // Define variáveis privadas que podem ser configuradas no Inspector do Unity.
    // Essas variáveis são referências para elementos de UI.
    [SerializeField] TextMeshProUGUI nicknameUI, roomNameUI, playerList;
    [SerializeField] Button joinButton, createButton, leaveButton, startButton;
    [SerializeField] GameObject menu, lobby;

    // Declara uma instância estática da classe MenuManager
    public static MenuManager instance;

    // Método chamado quando o script é inicializado.
    private void Awake()
    {
        #region Singleton
        // Verifica se a instância é nula
        if (instance == null)
        {
            instance = this; // Define a instância para este objeto
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Destroi o objeto se já houver uma instância existente
        }
        #endregion

        // Adiciona listeners aos botões, associando métodos aos eventos de clique.
        joinButton.onClick.AddListener(JoinRoom);
        createButton.onClick.AddListener(CreateRoom);
        leaveButton.onClick.AddListener(LeaveRoom);
        startButton.onClick.AddListener(delegate { StartGame("Jogo"); });

        // Define os botões joinButton e createButton como não interativos inicialmente.
        joinButton.interactable = false;
        createButton.interactable = false;

        // Chama o método SwitchWindow para exibir o menu inicial.
        SwitchWindow(false);
    }

    // Método público que habilita os botões joinButton e createButton quando conectado.
    public void Connected()
    {
        joinButton.interactable = true;
        createButton.interactable = true;
    }

    // Método público que atualiza a lista de jogadores na interface.
    public void UpdatePlayerList(string list)
    {
        playerList.text = list;
    }

    // Método privado que é chamado quando joinButton é clicado.
    private void JoinRoom()
    {
        // Chama o método JoinRoom no NetworkManager para se juntar a uma sala.
        NetworkManager.instance.JoinRoom(roomNameUI.text, nicknameUI.text);
        // Alterna para a janela do lobby.
        SwitchWindow(true);
    }

    // Método privado que é chamado quando createButton é clicado.
    private void CreateRoom()
    {
        // Chama o método CreateRoom no NetworkManager para criar uma sala.
        NetworkManager.instance.CreateRoom(roomNameUI.text, nicknameUI.text);
        // Alterna para a janela do lobby.
        SwitchWindow(true);
    }

    // Método privado que é chamado quando leaveButton é clicado.
    private void LeaveRoom()
    {
        // Chama o método LeaveRoom no NetworkManager para sair da sala.
        NetworkManager.instance.LeaveRoom();
        // Alterna para a janela do menu.
        SwitchWindow(false);
    }

    // Método público que inicia o jogo, carregando a cena especificada.
    public void StartGame(string sceneName)
    {
        NetworkManager.instance.LoadScene(sceneName);
    }

    // Método público que define se o botão startButton está interativo com base no valor de isMaster.
    public void SetStartButton(bool isMaster)
    {
        startButton.interactable = isMaster;
    }

    // Método público que alterna entre o menu e a janela do lobby.
    public void SwitchWindow(bool onLobby)
    {
        // Define a visibilidade dos GameObjects menu e lobby.
        menu.SetActive(!onLobby);
        lobby.SetActive(onLobby);
    }
}
