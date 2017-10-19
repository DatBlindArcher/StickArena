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
}