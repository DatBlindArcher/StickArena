using Steamworks;
using System.Collections.Generic;

public class Player
{
    public CSteamID ID;

    public Player() : this(CSteamID.Nil) { }
    public Player(CSteamID ID)
    {
        this.ID = ID;
    }
}

public class Lobby
{
    public CSteamID ID;

    public Lobby() : this(CSteamID.Nil) { }
    public Lobby(CSteamID ID)
    {
        this.ID = ID;
    }

    public void Update(out List<Player> playersJoined, out List<Player> playersLeft)
    {

    }
}
