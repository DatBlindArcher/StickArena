using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using ArcherNetwork;

public class Game : MonoBehaviour
{
    private GameInfo gameInfo;
    private Dictionary<CSteamID, PlayerController> players;

    public GameObject playerObj;
    public Transform[] spawnpoints;

    public void Initialize(GameInfo gameInfo)
    {
        this.gameInfo = gameInfo;
        Debug.Log(this.gameInfo.mode.ToString());
        players = new Dictionary<CSteamID, PlayerController>();
    }

    public void OnPlayerJoined(Player player)
    {
        if (players.ContainsKey(player.ID))
            return;

        PlayerController controller = Instantiate(playerObj).GetComponent<PlayerController>();
        controller.SetPlayer(player);
        players.Add(player.ID, controller);
    }

    public void OnPlayerLeft(Player player)
    {
        if (!players.ContainsKey(player.ID))
            return;

        players.Remove(player.ID);
    }

    public void OnHostChanged(Player player)
    {

    }

    public void OnPacketReceived(Player sender, NetworkBuffer packet)
    {

    }
}