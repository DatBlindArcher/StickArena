using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using ArcherNetwork;
using Steamworks;

public class NetworkBufferTest
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

        NetworkBuffer buffer = new NetworkBuffer();
        buffer.Write(b);
        buffer.Write(sb);
        buffer.Write(s);
        buffer.Write(us);
        buffer.Write(i);
        buffer.Write(ui);
        buffer.Write(l);
        buffer.Write(ul);
        buffer.Write(f);
        buffer.Write(d);
        buffer.Write(bo);
        buffer.Write(c);

        Assert.AreEqual(b, buffer.ReadByte());
        Assert.AreEqual(sb, buffer.ReadSByte());
        Assert.AreEqual(s, buffer.ReadShort());
        Assert.AreEqual(us, buffer.ReadUShort());
        Assert.AreEqual(i, buffer.ReadInt());
        Assert.AreEqual(ui, buffer.ReadUInt());
        Assert.AreEqual(l, buffer.ReadLong());
        Assert.AreEqual(ul, buffer.ReadULong());
        Assert.AreEqual(f, buffer.ReadFloat());
        Assert.AreEqual(d, buffer.ReadDouble());
        Assert.AreEqual(bo, buffer.ReadBool());
        Assert.AreEqual(c, buffer.ReadChar());

        buffer.ForceIndex(6, () => { Assert.AreEqual(i, buffer.ReadUInt()); });
    }

    public enum NetworkBufferTestType : short
    {
        Nan = 0,
        None = 128,
        Nill = 256
    }

    public class NetworkBufferTestObject : INetworkObject
    {
        public double d;

        public void Serialize(NetworkBuffer buffer)
        {
            buffer.Write(d);
        }

        public void Deserialize(NetworkBuffer buffer)
        {
            d = buffer.ReadDouble();
        }
    }

    [Test]
    public void Generic()
    {
        string s = "Hello World!";
        NetworkBufferTestType t = NetworkBufferTestType.Nill;
        NetworkBufferTestObject o = new NetworkBufferTestObject() { d = 56354.54 };
        int[] a = new int[1] { 256584 };
        List<int> l = new List<int>(a);
        List<NetworkBufferTestObject[]> overkill = new List<NetworkBufferTestObject[]>();
        overkill.Add(new NetworkBufferTestObject[1] { o });

        NetworkBuffer buffer = new NetworkBuffer();
        buffer.Write(s);
        buffer.Write(t);
        buffer.Write(o);
        buffer.Write(a);
        buffer.Write(l);
        buffer.Write(overkill);

        Assert.AreEqual(s, buffer.ReadString());
        Assert.AreEqual(t, (NetworkBufferTestType)buffer.ReadEnum(typeof(NetworkBufferTestType)));
        Assert.AreEqual(o.d, ((NetworkBufferTestObject)buffer.ReadNetworkObject(typeof(NetworkBufferTestObject))).d);
        Assert.AreEqual(a[0], ((int[])buffer.ReadList(typeof(int[])))[0]);
        Assert.AreEqual(a[0], ((List<int>)buffer.ReadList(typeof(List<int>)))[0]);
        Assert.AreEqual(overkill[0][0].d, ((List<NetworkBufferTestObject[]>)buffer.ReadList(typeof(List<NetworkBufferTestObject[]>)))[0][0].d);
    }

    [Test]
    public void Unity()
    {
        Vector2 v2 = new Vector2(4f, 5f);
        Vector3 v3 = new Vector3(4f, 5f, 56f);
        Vector4 v4 = new Vector4(4f, 5f, 56f, -3f);
        Quaternion q = new Quaternion(4f, 5f, 56f, -3f);

        NetworkBuffer buffer = new NetworkBuffer();
        buffer.Write(v2);
        buffer.Write(v3);
        buffer.Write(v4);
        buffer.Write(q);

        Assert.AreEqual(v2, buffer.ReadVector2());
        Assert.AreEqual(v3, buffer.ReadVector3());
        Assert.AreEqual(v4, buffer.ReadVector4());
        Assert.AreEqual(q, buffer.ReadQuaternion());
    }

    [Test]
    public void Custom()
    {
        CSteamID id = new CSteamID(654654651654);

        NetworkBuffer buffer = new NetworkBuffer();
        buffer.Write(id);

        Assert.AreEqual(id, buffer.ReadSteamID());
    }

    [Test]
    public void Performance()
    {
        for (int i = 0; i < 100; i++) Default();
        for (int i = 0; i < 100; i++) Generic();
        for (int i = 0; i < 100; i++) Unity();
    }
}