using Steamworks;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
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

    private bool steam = false;
    public IGame game;
    public Player player = new Player();
    public Lobby lobby = new Lobby();

    private CallResult<LobbyEnter_t> lobbyJoin;
    private CallResult<LobbyCreated_t> lobbyCreate;
    private CallResult<LobbyMatchList_t> lobbyListRequest;
    private Callback<LobbyChatMsg_t> lobbyMessage;
    private Callback<LobbyChatUpdate_t> lobbyUpdate;
    private Callback<LobbyDataUpdate_t> lobbyDataUpdate;
    private Callback<P2PSessionRequest_t> P2PSessionRequest;
    private Callback<P2PSessionConnectFail_t> P2PSessionConnectFail;

    private void Update()
    {
        if (steam)
            SteamAPI.RunCallbacks();
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
        InitSteam();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Arena");
    }

    #region Steam
    private void InitSteam()
    {
        if (steam)
        {
            throw new System.Exception("Tried to Initialize the SteamAPI twice in one session!");
        }

        if (!Packsize.Test())
        {
            Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
        }

        if (!DllCheck.Test())
        {
            Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
        }

        try
        {
            if (SteamAPI.RestartAppIfNecessary(AppId_t.Invalid))
            {
                Application.Quit();
                return;
            }
        }

        catch (System.DllNotFoundException e)
        {
            Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e, this);
            Application.Quit();
            return;
        }
        
        steam = SteamAPI.Init();

        if (!steam)
        {
            Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
            return;
        }

        player = new Player(SteamUser.GetSteamID());
        lobbyJoin = CallResult<LobbyEnter_t>.Create(OnLobbyJoined);
        lobbyCreate = CallResult<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyListRequest = CallResult<LobbyMatchList_t>.Create(OnLobbyList);
        lobbyMessage = Callback<LobbyChatMsg_t>.Create(OnLobbyMessage);
        lobbyUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyUpdated);
        lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdated);
        P2PSessionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
        P2PSessionConnectFail = Callback<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectFail);
        Debug.Log("Steam is initialized.");
    }

    private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
    private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
    {
        Debug.LogWarning(pchDebugText);
    }

    private void OnEnable()
    {
        if (Instance == null)
            Instance = this;

        if (!steam)
            return;

        if (m_SteamAPIWarningMessageHook == null)
        {
            m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
            SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
        }
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        Instance = null;

        if (!steam)
            return;

        if (lobby.ID.IsValid())
            LeaveLobby();

        SteamAPI.Shutdown();
    }

    public void FindLobby()
    {
        SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide);
        SteamMatchmaking.AddRequestLobbyListStringFilter("Game", "StickArena", ELobbyComparison.k_ELobbyComparisonEqual);
        lobbyListRequest.Set(SteamMatchmaking.RequestLobbyList());
        Debug.Log("Looking for lobbies");
    }

    public void JoinLobby(CSteamID lobbyID)
    {
        lobbyJoin.Set(SteamMatchmaking.JoinLobby(lobbyID));
    }

    public void CreateLobby()
    {
        lobbyCreate.Set(SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4));
    }

    public void LeaveLobby()
    {
        if (lobby.ID.IsValid())
        {
            SteamMatchmaking.LeaveLobby(lobby.ID);
            lobby = new Lobby();
            Debug.Log("Left lobby.");
        }
    }

    private void OnLobbyList(LobbyMatchList_t callback, bool failed)
    {
        if (failed)
        {
            Debug.LogError("Requesting lobbylist failed.");
            return;
        }

        Debug.Log("Current Lobbies: " + callback.m_nLobbiesMatching);

        for (int i = 0; i < callback.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyListID = SteamMatchmaking.GetLobbyByIndex(i);

            if (lobbyListID.IsValid())
            {
                JoinLobby(lobbyListID);
                return;
            }
        }

        CreateLobby();
    }

    private void OnLobbyJoined(LobbyEnter_t callback, bool failed)
    {
        if (failed)
        {
            Debug.LogError("Joining lobby failed.");
            return;
        }

        Debug.Log("Lobby Joined: " + callback.m_ulSteamIDLobby);
        lobby = new Lobby((CSteamID)callback.m_ulSteamIDLobby);
    }

    private void OnLobbyCreated(LobbyCreated_t callback, bool failed)
    {
        if (failed)
        {
            Debug.LogError("Creating lobby failed.");
            return;
        }

        if (!SteamMatchmaking.RequestLobbyData((CSteamID)callback.m_ulSteamIDLobby))
        {
            Debug.LogError("Failed to retrieve lobby data.");
        }

        lobby.ID = (CSteamID)callback.m_ulSteamIDLobby;
        //SteamMatchmaking.SetLobbyData((CSteamID)callback.m_ulSteamIDLobby, "Title", lobby.name);
        SteamMatchmaking.SetLobbyData((CSteamID)callback.m_ulSteamIDLobby, "Game", "StickArena");
        Debug.Log("Lobby created: " + callback.m_ulSteamIDLobby);
    }

    private void OnLobbyDataUpdated(LobbyDataUpdate_t callback)
    {
        Debug.Log("Lobby data updated: " + callback.m_ulSteamIDLobby);

        if ((ulong)lobby.ID == callback.m_ulSteamIDLobby)
            lobby.Update();
    }

    private void OnLobbyUpdated(LobbyChatUpdate_t callback)
    {
        Debug.Log("Lobby updated: " + callback.m_ulSteamIDLobby);

        if ((ulong)lobby.ID == callback.m_ulSteamIDLobby)
        {
            lobby.Update((p) =>
            { // Player p Joined
                Debug.Log("Lobby updated: ");
            }, (p) => 
            { // Player p Left

            });
        }
    }

    public void SendLobbyMessage(byte[] buffer)
    {
        if (!SteamMatchmaking.SendLobbyChatMsg(lobby.ID, buffer, buffer.Length))
        {
            Debug.LogError("Failed to send message.");
        }
    }

    private void OnLobbyMessage(LobbyChatMsg_t callback)
    {
        CSteamID sender;
        byte[] buffer = new byte[1024];
        EChatEntryType type;
        int length = SteamMatchmaking.GetLobbyChatEntry((CSteamID)callback.m_ulSteamIDLobby, (int)callback.m_iChatID, out sender, buffer, 1024, out type);
        byte[] result = new byte[length];
        System.Array.Copy(buffer, result, length);
    }

    private void OnP2PSessionRequest(P2PSessionRequest_t callback)
    {
        Debug.Log("OnP2PSesssionRequest Called steamIDRemote: " + callback.m_steamIDRemote);

        if (!SteamNetworking.AcceptP2PSessionWithUser(callback.m_steamIDRemote))
        {
            Debug.Log("OnP2PSesssionRequest Invalid steamIDRemote: " + callback.m_steamIDRemote);
        }
    }

    private void OnP2PSessionConnectFail(P2PSessionConnectFail_t callback)
    {
        Debug.Log("OnP2PSessionConnectFail Called steamIDRemote: " + callback.m_steamIDRemote);
    }

    private void Ignore()
    {
        lobbyListRequest.ToString();
        lobbyMessage.ToString();
        lobbyUpdate.ToString();
        lobbyDataUpdate.ToString();
        P2PSessionRequest.ToString();
        P2PSessionConnectFail.ToString();
    }
    #endregion
}