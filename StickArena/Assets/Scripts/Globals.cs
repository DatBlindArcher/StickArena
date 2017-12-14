using ArcherNetwork;
using UnityEngine;

public enum NetworkTarget : byte
{
    All = 0,
    Others = 1,
    Host = 2,
    Single = 3,
    Buffered = 4
}

public enum SendType : byte
{
    SlowButReliable,
    FastButUnreliable
}

public enum PacketType : byte
{
    State = 0,

    StartGame = 100
}


public class PlayerState : INetworkObject
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

    public void Serialize(Packet packet)
    {
        packet.Write(timestamp);
        packet.Write(pos);
        packet.Write(cam);
        packet.Write(rot);
    }

    public void Deserialize(Packet packet)
    {
        timestamp = packet.ReadFloat();
        pos = packet.ReadVector2();
        cam = packet.ReadVector2();
        rot = packet.ReadFloat();
    }
}