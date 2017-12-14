using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using ArcherNetwork;
using Steamworks;

public class PacketTest
{
    [Test]
    public void Default()
    {
        byte b = 65;
        sbyte sb = -5;
        short s = 85;
        ushort us = 654;
        int i = 5646;
        uint ui = 5465468;
        long l = 5645635365;
        ulong ul = 5463546354;
        float f = 55f;
        double d = 5.5;
        bool bo = false;
        char c = '\n';

        Packet packet = new Packet();
        packet.Write(b);
        packet.Write(sb);
        packet.Write(s);
        packet.Write(us);
        packet.Write(i);
        packet.Write(ui);
        packet.Write(l);
        packet.Write(ul);
        packet.Write(f);
        packet.Write(d);
        packet.Write(bo);
        packet.Write(c);

        packet = new Packet(packet.GetBytes());
        Assert.AreEqual(b, packet.ReadByte());
        Assert.AreEqual(sb, packet.ReadSByte());
        Assert.AreEqual(s, packet.ReadShort());
        Assert.AreEqual(us, packet.ReadUShort());
        Assert.AreEqual(i, packet.ReadInt());
        Assert.AreEqual(ui, packet.ReadUInt());
        Assert.AreEqual(l, packet.ReadLong());
        Assert.AreEqual(ul, packet.ReadULong());
        Assert.AreEqual(f, packet.ReadFloat());
        Assert.AreEqual(d, packet.ReadDouble());
        Assert.AreEqual(bo, packet.ReadBool());
        Assert.AreEqual(c, packet.ReadChar());
    }

    public enum PacketTestType : short
    {
        Nan = 0,
        None = 128,
        Nill = 256
    }

    public class PacketTestObject : INetworkObject
    {
        public double d;

        public void Serialize(Packet packet)
        {
            packet.Write(d);
        }

        public void Deserialize(Packet packet)
        {
            d = packet.ReadDouble();
        }
    }

    [Test]
    public void Generic()
    {
        string s = "Hello World!";
        PacketTestType t = PacketTestType.Nill;
        PacketTestObject o = new PacketTestObject() { d = 56354.54 };
        int[] a = new int[1] { 256584 };
        List<int> l = new List<int>(a);
        List<PacketTestObject[]> overkill = new List<PacketTestObject[]>();
        overkill.Add(new PacketTestObject[1] { o });

        Packet packet = new Packet();
        packet.Write(s);
        packet.Write(t);
        packet.Write(o);
        packet.Write(a);
        packet.Write(l);
        packet.Write(overkill);

        packet = new Packet(packet.GetBytes());
        Assert.AreEqual(s, packet.ReadString());
        Assert.AreEqual(t, packet.ReadEnum<PacketTestType>());
        Assert.AreEqual(o.d, packet.ReadNetworkObject<PacketTestObject>().d);
        Assert.AreEqual(a[0], packet.ReadList<int[]>()[0]);
        Assert.AreEqual(a[0], packet.ReadList<List<int>>()[0]);
        Assert.AreEqual(overkill[0][0].d, packet.ReadList<List<PacketTestObject[]>>()[0][0].d);
    }

    [Test]
    public void Unity()
    {
        Vector2 v2 = new Vector2(4f, 5f);
        Vector3 v3 = new Vector3(4f, 5f, 56f);
        Vector4 v4 = new Vector4(4f, 5f, 56f, -3f);
        Quaternion q = new Quaternion(4f, 5f, 56f, -3f);

        Packet packet = new Packet();
        packet.Write(v2);
        packet.Write(v3);
        packet.Write(v4);
        packet.Write(q);
        
        packet = new Packet(packet.GetBytes());
        Assert.AreEqual(v2, packet.ReadVector2());
        Assert.AreEqual(v3, packet.ReadVector3());
        Assert.AreEqual(v4, packet.ReadVector4());
        Assert.AreEqual(q, packet.ReadQuaternion());
    }

    [Test]
    public void Custom()
    {
        CSteamID id = new CSteamID(654654651654);

        Packet packet = new Packet();
        packet.Write(id);

        packet = new Packet(packet.GetBytes());
        Assert.AreEqual(id, packet.ReadSteamID());
    }

    [Test]
    public void Performance()
    {
        for (int i = 0; i < 100; i++) Default();
        for (int i = 0; i < 100; i++) Generic();
        for (int i = 0; i < 100; i++) Unity();
    }
}