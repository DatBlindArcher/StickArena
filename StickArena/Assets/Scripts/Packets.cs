using ArcherNetwork;
using UnityEngine;

public enum NetworkTarget : byte
{
    All = 0,
    Host = 1,
    Single = 2,
    Buffered = 3
}

public enum SendType : byte
{
    FastButReliable,
    SlowButUnreliable
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

    public void Serialize(NetworkBuffer buffer)
    {
        buffer.Write(timestamp);
        buffer.Write(pos);
        buffer.Write(cam);
        buffer.Write(rot);
    }

    public void Deserialize(NetworkBuffer buffer)
    {
        timestamp = buffer.ReadFloat();
        pos = buffer.ReadVector2();
        cam = buffer.ReadVector2();
        rot = buffer.ReadFloat();
    }
}