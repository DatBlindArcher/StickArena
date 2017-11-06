using UnityEngine;

public class Packet
{
    public byte type;
    public byte[] data;
}

public class PlayerState
{
    public float timestamp;
    public Vector2 pos;
    public Vector2 cam;
    public float rot;

    public PlayerState copy
    {
        get
        {
            return new PlayerState() { timestamp = timestamp, pos = pos, cam = cam, rot = rot };
        }
    }
}

public interface IGame
{
    void StartGame();
    void EndGame();
    void OnPlayerJoined(Player player);
    void OnPlayerLeft(Player player);
    void OnPlayerUpdated(Player player);
    void OnHostChanged(Player player);
    void OnPacketReceived(Packet packet);
}