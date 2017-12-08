using System;
using System.Collections.Generic;
using Steamworks;
using ArcherNetwork;

public enum GameState
{
    Menu,
    Lobby,
    Game
}

public enum GameMode
{
    DeathMatch
}

public enum GameMap
{
    Arena
}

public class GameInfo : INetworkObject
{
    public GameMode mode;
    public GameMap map;

    public void Serialize(NetworkBuffer buffer)
    {
        buffer.Write(mode);
        buffer.Write(map);
    }

    public void Deserialize(NetworkBuffer buffer)
    {
        mode = (GameMode)buffer.ReadEnum(typeof(GameMode));
        map = (GameMap)buffer.ReadEnum(typeof(GameMap));
    }
}

public class Player
{
    public CSteamID ID;
    public string name;

    public Player() : this(CSteamID.Nil) { }
    public Player(CSteamID ID)
    {
        this.ID = ID;
        Update();
    }

    public void Update()
    {
        if (this == GameController.instance.player) name = SteamFriends.GetPersonaName();
        else name = SteamFriends.GetFriendPersonaName(ID);
    }
}

public class Lobby
{
    public CSteamID ID;
    public string name;
    public int maxPlayers;
    public Player host;
    public Dictionary<CSteamID, Player> players;

    public bool isHost
    {
        get
        {
            return host.ID == GameController.instance.player.ID;
        }
    }

    public Lobby() : this(CSteamID.Nil) { }
    public Lobby(CSteamID ID)
    {
        this.ID = ID;
        players = new Dictionary<CSteamID, Player>();
        Update();
    }

    public void Update() { Update((p) => { }, (p) => { }); }
    public void Update(Action<Player> playerJoined, Action<Player> playerLeft)
    {
        name = SteamMatchmaking.GetLobbyData(ID, "Title");
        int playerCount = SteamMatchmaking.GetNumLobbyMembers(ID);
        maxPlayers = SteamMatchmaking.GetLobbyMemberLimit(ID);
        HashSet<CSteamID> playersToUpdate = new HashSet<CSteamID>();

        for (int i = 0; i < playerCount; i++)
        {
            playersToUpdate.Add(SteamMatchmaking.GetLobbyMemberByIndex(ID, i));
        }

        foreach (CSteamID id in players.Keys)
        {
            if (!playersToUpdate.Contains(id))
            {
                playerLeft(players[id]);
                players.Remove(id);
            }

            else
            {
                players[id].Update();
                playersToUpdate.Remove(id);
            }
        }

        foreach (CSteamID id in playersToUpdate)
        {
            players.Add(id, new Player(id));
            playerJoined(players[id]);
        }

        playersToUpdate.Clear();
        host = players[SteamMatchmaking.GetLobbyOwner(ID)];
    }

    public void Leave()
    {
        if (ID.IsValid())
        {
            SteamMatchmaking.LeaveLobby(ID);
        }
    }
}