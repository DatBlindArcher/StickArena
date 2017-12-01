using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ArcherNetwork;
using Steamworks;

// TODO: 
// - buffered packets
// - make a test
public class GameController : MonoBehaviour, ISteamController
{
    #region Setup
    private static GameController Instance;
    public static GameController instance
    {
        get
        {
            if (Instance == null)
                return new GameObject("GameController").AddComponent<GameController>();
            return Instance;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        steam = Steam.Initialize(this);
    }

    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        Instance = null;
    }
    #endregion

    public Game game;
    public Steam steam;
    public Player player;
    public Lobby lobby;

    public void StartGame(GameInfo info)
    {
        StartCoroutine(LoadScene(info));
    }

    private IEnumerator LoadScene(GameInfo info)
    {
        AsyncOperation a = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Single);
        yield return a;
        LoadingScreen loading = FindObjectOfType<LoadingScreen>();
        DontDestroyOnLoad(loading.gameObject);
        a = SceneManager.LoadSceneAsync(info.map.ToString(), LoadSceneMode.Single);

        while (a.progress < 1f)
        {
            loading.progress.text = "Loading " + a.progress.ToString("F") + "%";
            yield return new WaitForEndOfFrame();
        }

        yield return a;
        
        Game game = FindObjectOfType<Game>();
        game.Initialize(info);

        foreach (Player player in lobby.players.Values)
            game.OnPlayerJoined(player);

        DestroyImmediate(loading.gameObject);
    }

    public void SendPacket(SendType sendType, NetworkTarget target, NetworkBuffer buffer)
    {
        SendPacket(sendType, target, player.ID, CSteamID.Nil, buffer);
    }

    public void SendPacket(SendType sendType, CSteamID receiver, NetworkBuffer buffer)
    {
        SendPacket(sendType, NetworkTarget.Single, player.ID, receiver, buffer);
    }

    private void SendPacket(SendType sendType, NetworkTarget target, CSteamID sender, CSteamID receiver, NetworkBuffer buffer)
    {
        byte[] bytes = buffer.getBytes();
        EP2PSend sendtype = sendType == SendType.FastButReliable ? EP2PSend.k_EP2PSendReliable : EP2PSend.k_EP2PSendUnreliableNoDelay;

        NetworkBuffer finalBuffer = new NetworkBuffer();
        finalBuffer.Write(sendType);
        finalBuffer.Write(target);
        finalBuffer.Write(sender);
        finalBuffer.Write(receiver);
        finalBuffer.Write(bytes);
        byte[] data = finalBuffer.getBytes();

        if (lobby.isHost)
        {
            switch (target)
            {
                case NetworkTarget.All:
                case NetworkTarget.Buffered:
                    foreach (CSteamID player in lobby.players.Keys)
                        if (player != lobby.host.ID)
                            steam.Send(player, data, sendtype);
                    if (player.ID == sender)
                        OnPacket(data);
                    break;

                case NetworkTarget.Host:
                    OnPacket(data);
                    break;

                case NetworkTarget.Single:
                    if (receiver == lobby.host.ID)
                        OnPacket(data);
                    else
                        steam.Send(receiver, data, sendtype);
                    break;
            }
        }

        else
        {
            steam.Send(lobby.host.ID, data, sendtype);
        }
    }

    public void OnPacket(byte[] data)
    {
        NetworkBuffer firstBuffer = new NetworkBuffer(data);
        SendType sendType = (SendType)firstBuffer.ReadEnum(typeof(SendType));
        NetworkTarget target = (NetworkTarget)firstBuffer.ReadEnum(typeof(NetworkTarget));
        CSteamID sender = firstBuffer.ReadSteamID();
        CSteamID receiver = firstBuffer.ReadSteamID();
        byte[] bytes = (byte[])firstBuffer.ReadList(typeof(byte[]));
        NetworkBuffer buffer = new NetworkBuffer(bytes);

        if (target == NetworkTarget.Buffered)
        {
            // buffer it
        }

        if (lobby.isHost)
        {
            switch (target)
            {
                case NetworkTarget.Buffered:
                case NetworkTarget.All:
                    game.OnPacketReceived(lobby.players[sender], buffer);
                    if (player.ID != sender) SendPacket(sendType, target, sender, receiver, buffer);
                    break;

                case NetworkTarget.Host:
                    game.OnPacketReceived(lobby.players[sender], buffer);
                    break;

                case NetworkTarget.Single:
                    if (player.ID == receiver) game.OnPacketReceived(lobby.players[sender], buffer);
                    else SendPacket(sendType, target, sender, receiver, buffer);
                    break;
            }
        }

        else
        {
            game.OnPacketReceived(lobby.players[sender], buffer);
        }
    }

    public void OnSteamInitialized(Player player)
    {
        this.player = player;
        Debug.Log("Steam initialized");
    }

    public void OnLobbyJoined(Lobby lobby)
    {
        this.lobby = lobby;
        Debug.Log("Lobby joined");
    }

    public void OnLobbyCreated(Lobby lobby)
    {
        this.lobby = lobby;
        Debug.Log("Lobby created");
    }

    public void OnLobbyUpdated()
    {
        Debug.Log("Lobby updated");
    }

    public void OnPlayerJoined(Player player)
    {
        if (game != null) game.OnPlayerJoined(player);
        Debug.Log("Player " + player.name + " has joined.");
    }

    public void OnPlayerLeft(Player player)
    {
        if (game != null) game.OnPlayerLeft(player);
        Debug.Log("Player " + player.name + " has left.");
    }

    public void OnLobbyMessage(byte[] buffer)
    {
        Debug.Log("Lobby message received.");
    }
}