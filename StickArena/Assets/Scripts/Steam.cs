using System.Text;
using UnityEngine;
using Steamworks;
using System;

public interface ISteamController
{
    void OnSteamInitialized(Player player);

    void OnLobbyJoined(Lobby lobby);
    void OnLobbyCreated(Lobby lobby);
    void OnLobbyUpdated();

    void OnPlayerJoined(Player player);
    void OnPlayerLeft(Player player);

    void OnLobbyMessage(byte[] buffer);
    void OnPacket(byte[] buffer);
}

public class Steam : MonoBehaviour
{
    public static Steam isntance { get; private set; }

    private bool running;
    private Lobby lobby;
    private ISteamController controller;

    private CallResult<LobbyEnter_t> lobbyJoin;
    private CallResult<LobbyCreated_t> lobbyCreate;
    private CallResult<LobbyMatchList_t> lobbyListRequest;
    private Callback<LobbyChatMsg_t> lobbyMessage;
    private Callback<LobbyChatUpdate_t> lobbyUpdate;
    private Callback<LobbyDataUpdate_t> lobbyDataUpdate;
    private Callback<P2PSessionRequest_t> P2PSessionRequest;
    private Callback<P2PSessionConnectFail_t> P2PSessionConnectFail;

    public static Steam Initialize(ISteamController controller)
    {
        if (isntance != null)
            return null;

        GameObject obj = new GameObject("Steam");
        Steam s = obj.AddComponent<Steam>();
        s.Init(controller);
        isntance = s;
        DontDestroyOnLoad(obj);
        return s;
    }

    private void Init(ISteamController controller)
    {
        running = false;
        this.controller = controller;

        if (running)
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

        running = SteamAPI.Init();

        if (!running)
        {
            Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
            return;
        }

        lobbyJoin = CallResult<LobbyEnter_t>.Create(OnLobbyJoined);
        lobbyCreate = CallResult<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyListRequest = CallResult<LobbyMatchList_t>.Create(OnLobbyList);
        lobbyMessage = Callback<LobbyChatMsg_t>.Create(OnLobbyMessage);
        lobbyUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyUpdated);
        lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdated);
        P2PSessionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
        P2PSessionConnectFail = Callback<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectFail);
        controller.OnSteamInitialized(new Player(SteamUser.GetSteamID()));
    }

    public void Send(CSteamID receiver, byte[] buffer, EP2PSend sendType)
    {
        SteamNetworking.SendP2PPacket(receiver, buffer, (uint)buffer.Length, sendType);
    }

    private void Update()
    {
        if (running)
            SteamAPI.RunCallbacks();
    }

    private void OnEnable()
    {
        if (m_SteamAPIWarningMessageHook == null)
        {
            m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
            SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
        }
    }

    private void OnDestroy()
    {
        if (!running)
            return;

        if (lobby.ID.IsValid())
        {
            lobby.Leave();
            lobby = null;
        }

        SteamAPI.Shutdown();
    }

    private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;
    private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
    {
        Debug.LogWarning(pchDebugText);
    }

    public void FindLobby()
    {
        SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide);
        SteamMatchmaking.AddRequestLobbyListStringFilter("Game", "StickArena", ELobbyComparison.k_ELobbyComparisonEqual);
        lobbyListRequest.Set(SteamMatchmaking.RequestLobbyList());
    }

    public void JoinLobby(CSteamID lobbyID)
    {
        lobbyJoin.Set(SteamMatchmaking.JoinLobby(lobbyID));
    }

    public void CreateLobby()
    {
        lobbyCreate.Set(SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4));
    }

    private void OnLobbyList(LobbyMatchList_t callback, bool failed)
    {
        if (failed)
        {
            Debug.LogError("Requesting lobbylist failed.");
            return;
        }

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
        
        controller.OnLobbyJoined(new Lobby((CSteamID)callback.m_ulSteamIDLobby));
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

        CSteamID ID = (CSteamID)callback.m_ulSteamIDLobby;
        //SteamMatchmaking.SetLobbyData((CSteamID)callback.m_ulSteamIDLobby, "Title", lobby.name);
        SteamMatchmaking.SetLobbyData(ID, "Game", "StickArena");
        controller.OnLobbyCreated(new Lobby(ID));
    }

    private void OnLobbyDataUpdated(LobbyDataUpdate_t callback)
    {
        if ((ulong)lobby.ID == callback.m_ulSteamIDLobby)
        {
            lobby.Update();
            controller.OnLobbyUpdated();
        }
    }

    private void OnLobbyUpdated(LobbyChatUpdate_t callback)
    {
        if ((ulong)lobby.ID == callback.m_ulSteamIDLobby)
        {
            lobby.Update(controller.OnPlayerJoined, controller.OnPlayerLeft);
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
        controller.OnLobbyMessage(result);
    }

    private void OnP2PSessionRequest(P2PSessionRequest_t callback)
    {
        if (!SteamNetworking.AcceptP2PSessionWithUser(callback.m_steamIDRemote))
        {
            Debug.LogError("OnP2PSesssionRequest Invalid steamIDRemote: " + callback.m_steamIDRemote);
        }
    }

    private void OnP2PSessionConnectFail(P2PSessionConnectFail_t callback)
    {
        Debug.LogError("OnP2PSessionConnectFail Called steamIDRemote: " + callback.m_steamIDRemote);
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
}