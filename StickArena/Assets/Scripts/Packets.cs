using UnityEngine;

public class Packet
{
    public byte type;
    public byte[] data;
}

[System.Serializable]
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