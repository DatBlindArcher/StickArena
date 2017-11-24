using Steamworks;
using System;
using System.Collections.Generic;

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

    public Lobby() : this(CSteamID.Nil) { }
    public Lobby(CSteamID ID)
    {
        this.ID = ID;
        players = new Dictionary<CSteamID, Player>();
        Update();
    }

    public void Update() { Update(null, null); }
    public void Update(Action<Player> playerJoined, Action<Player> playerLeft)
    {
        name = SteamMatchmaking.GetLobbyData(ID, "Title");
        int playerCount = SteamMatchmaking.GetNumLobbyMembers(ID);
        maxPlayers = SteamMatchmaking.GetLobbyMemberLimit(ID);

        for (int i = 0; i < playerCount; i++)
        {
            CSteamID playerID = SteamMatchmaking.GetLobbyMemberByIndex(ID, i);
            if (!players.ContainsKey(ID))
            {
                Player p = new Player(playerID);
                players.Add(playerID, p);
                playerJoined(p);
            }
        }

        // Check if dictionary has more players than the amount

        host = players[ID];
    }
}
