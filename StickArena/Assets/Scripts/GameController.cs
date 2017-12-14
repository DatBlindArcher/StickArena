using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ArcherNetwork;
using Steamworks;

// TODO: 
// - buffered packets must be sent to new players
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

    private List<byte[]> bufferPackets;

    public GameState getState
    {
        get
        {
            if (lobby != null && lobby.ID.IsValid())
            {
                if (game != null)
                    return GameState.Game;
                return GameState.Lobby;
            }

            return GameState.Menu;
        }
    }

    private void Start()
    {
        bufferPackets = new List<byte[]>();
    }

    public void StartGame(GameInfo info)
    {
        if (lobby.isHost)
        {
            Packet packet = new Packet();
            packet.Write(PacketType.StartGame);
            packet.Write(info);
            steam.SendLobbyMessage(packet.GetBytes());
        }
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
        
        game = FindObjectOfType<Game>();
        game.Initialize(info);

        foreach (Player player in lobby.players.Values)
            game.OnPlayerJoined(player);

        DestroyImmediate(loading.gameObject);
    }

    public void SendPacket(SendType sendType, NetworkTarget target, Packet packet)
    {
        SendPacket(sendType, target, player.ID, CSteamID.Nil, packet);
    }

    public void SendPacket(SendType sendType, CSteamID receiver, Packet packet)
    {
        SendPacket(sendType, NetworkTarget.Single, player.ID, receiver, packet);
    }

    private void SendPacket(SendType sendType, NetworkTarget target, CSteamID sender, CSteamID receiver, Packet packet)
    {
        byte[] bytes = packet.GetBytes();
        EP2PSend sendtype = sendType == SendType.SlowButReliable ? EP2PSend.k_EP2PSendReliable : EP2PSend.k_EP2PSendUnreliableNoDelay;

        Packet parent = new Packet();
        parent.Write(lobby.isHost);
        parent.Write(sendType);
        parent.Write(target);
        parent.Write(sender);
        parent.Write(receiver);
        parent.Write(bytes);
        byte[] data = parent.GetBytes();

        if (lobby.isHost)
        {
            switch (target)
            {
                case NetworkTarget.All:
                case NetworkTarget.Buffered:
                    foreach (CSteamID player in lobby.players.Keys)
                        if (player != this.player.ID)
                            steam.Send(player, data, sendtype);
                    OnPacket(data);
                    break;

                case NetworkTarget.Others:
                    foreach (CSteamID player in lobby.players.Keys)
                        if (player != this.player.ID && player != sender)
                            steam.Send(player, data, sendtype);
                    if (sender != player.ID)
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
        Packet parent = new Packet(data);
        bool fromHost = parent.ReadBool();
        SendType sendType = parent.ReadEnum<SendType>();
        NetworkTarget target = parent.ReadEnum<NetworkTarget>();
        CSteamID sender = parent.ReadSteamID();
        CSteamID receiver = parent.ReadSteamID();
        byte[] bytes = parent.ReadList<byte[]>();
        Packet packet = new Packet(bytes);
        
        if (target == NetworkTarget.Buffered)
        {
            bufferPackets.Add(data);
        }

        if (!fromHost)
        {
            switch (target)
            {
                case NetworkTarget.Single:
                    if (player.ID != receiver) SendPacket(sendType, target, sender, receiver, packet);
                    break;

                default:
                    SendPacket(sendType, target, sender, receiver, packet);
                    break;
            }
        }

        if (game != null)
        {
            switch (target)
            {
                default:
                    game.OnPacketReceived(lobby.players[sender], packet);
                    break;

                case NetworkTarget.Single:
                    if (player.ID == receiver)
                        game.OnPacketReceived(lobby.players[sender], packet);
                    break;
            }
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
        Packet packet = new Packet(buffer);
        PacketType type = packet.ReadEnum<PacketType>();

        switch (type)
        {
            case PacketType.StartGame:
                StartCoroutine(LoadScene(packet.ReadNetworkObject<GameInfo>()));
                break;

            default:
                Debug.Log("Lobby message received.");
                break;
        }
    }
}