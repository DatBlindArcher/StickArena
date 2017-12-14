using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

namespace ArcherNetwork
{
    public partial class Packet
    {
        private static ByteStruct bs = new ByteStruct();
        private MemoryStream stream;

        public Packet()
        {
            stream = new MemoryStream();
        }

        public Packet(byte[] bytes)
        {
            stream = new MemoryStream(bytes);
        }

        public byte[] GetBytes()
        {
            return stream.GetBuffer();
        }

        public void Write(byte o)
        {
            stream.WriteByte(o);
        }

        public void Write(sbyte o)
        {
            Write((byte)o);
        }

        public void Write(short o)
        {
            bs.s = o;

            if (BitConverter.IsLittleEndian)
            {
                Write(bs.b2);
                Write(bs.b1);
            }

            else
            {
                Write(bs.b1);
                Write(bs.b2);
            }
        }

        public void Write(ushort o)
        {
            bs.us = o;

            if (BitConverter.IsLittleEndian)
            {
                Write(bs.b2);
                Write(bs.b1);
            }

            else
            {
                Write(bs.b1);
                Write(bs.b2);
            }
        }

        public void Write(int o)
        {
            bs.i = o;

            if (BitConverter.IsLittleEndian)
            {
                Write(bs.b4);
                Write(bs.b3);
                Write(bs.b2);
                Write(bs.b1);
            }

            else
            {
                Write(bs.b1);
                Write(bs.b2);
                Write(bs.b3);
                Write(bs.b4);
            }
        }

        public void Write(uint o)
        {
            bs.ui = o;

            if (BitConverter.IsLittleEndian)
            {
                Write(bs.b4);
                Write(bs.b3);
                Write(bs.b2);
                Write(bs.b1);
            }

            else
            {
                Write(bs.b1);
                Write(bs.b2);
                Write(bs.b3);
                Write(bs.b4);
            }
        }

        public void Write(long o)
        {
            bs.l = o;

            if (BitConverter.IsLittleEndian)
            {
                Write(bs.b8);
                Write(bs.b7);
                Write(bs.b6);
                Write(bs.b5);
                Write(bs.b4);
                Write(bs.b3);
                Write(bs.b2);
                Write(bs.b1);
            }

            else
            {
                Write(bs.b1);
                Write(bs.b2);
                Write(bs.b3);
                Write(bs.b4);
                Write(bs.b5);
                Write(bs.b6);
                Write(bs.b7);
                Write(bs.b8);
            }
        }

        public void Write(ulong o)
        {
            bs.ul = o;

            if (BitConverter.IsLittleEndian)
            {
                Write(bs.b8);
                Write(bs.b7);
                Write(bs.b6);
                Write(bs.b5);
                Write(bs.b4);
                Write(bs.b3);
                Write(bs.b2);
                Write(bs.b1);
            }

            else
            {
                Write(bs.b1);
                Write(bs.b2);
                Write(bs.b3);
                Write(bs.b4);
                Write(bs.b5);
                Write(bs.b6);
                Write(bs.b7);
                Write(bs.b8);
            }
        }

        public void Write(float o)
        {
            bs.f = o;

            if (BitConverter.IsLittleEndian)
            {
                Write(bs.b4);
                Write(bs.b3);
                Write(bs.b2);
                Write(bs.b1);
            }

            else
            {
                Write(bs.b1);
                Write(bs.b2);
                Write(bs.b3);
                Write(bs.b4);
            }
        }

        public void Write(double o)
        {
            bs.d = o;

            if (BitConverter.IsLittleEndian)
            {
                Write(bs.b8);
                Write(bs.b7);
                Write(bs.b6);
                Write(bs.b5);
                Write(bs.b4);
                Write(bs.b3);
                Write(bs.b2);
                Write(bs.b1);
            }

            else
            {
                Write(bs.b1);
                Write(bs.b2);
                Write(bs.b3);
                Write(bs.b4);
                Write(bs.b5);
                Write(bs.b6);
                Write(bs.b7);
                Write(bs.b8);
            }
        }

        public void Write(bool o)
        {
            Write((byte)(o ? 1 : 0));
        }

        public void Write(char o)
        {
            Write(Convert.ToByte(o));
        }

        public byte ReadByte()
        {
            return (byte)stream.ReadByte();
        }

        public sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }

        public short ReadShort()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b2 = ReadByte();
                bs.b1 = ReadByte();
            }

            else
            {
                bs.b1 = ReadByte();
                bs.b2 = ReadByte();
            }

            return bs.s;
        }

        public ushort ReadUShort()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b2 = ReadByte();
                bs.b1 = ReadByte();
            }

            else
            {
                bs.b1 = ReadByte();
                bs.b2 = ReadByte();
            }

            return bs.us;
        }

        public int ReadInt()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b4 = ReadByte();
                bs.b3 = ReadByte();
                bs.b2 = ReadByte();
                bs.b1 = ReadByte();
            }

            else
            {
                bs.b1 = ReadByte();
                bs.b2 = ReadByte();
                bs.b3 = ReadByte();
                bs.b4 = ReadByte();
            }

            return bs.i;
        }

        public uint ReadUInt()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b4 = ReadByte();
                bs.b3 = ReadByte();
                bs.b2 = ReadByte();
                bs.b1 = ReadByte();
            }

            else
            {
                bs.b1 = ReadByte();
                bs.b2 = ReadByte();
                bs.b3 = ReadByte();
                bs.b4 = ReadByte();
            }

            return bs.ui;
        }

        public long ReadLong()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b8 = ReadByte();
                bs.b7 = ReadByte();
                bs.b6 = ReadByte();
                bs.b5 = ReadByte();
                bs.b4 = ReadByte();
                bs.b3 = ReadByte();
                bs.b2 = ReadByte();
                bs.b1 = ReadByte();
            }

            else
            {
                bs.b1 = ReadByte();
                bs.b2 = ReadByte();
                bs.b3 = ReadByte();
                bs.b4 = ReadByte();
                bs.b5 = ReadByte();
                bs.b6 = ReadByte();
                bs.b7 = ReadByte();
                bs.b8 = ReadByte();
            }

            return bs.l;
        }

        public ulong ReadULong()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b8 = ReadByte();
                bs.b7 = ReadByte();
                bs.b6 = ReadByte();
                bs.b5 = ReadByte();
                bs.b4 = ReadByte();
                bs.b3 = ReadByte();
                bs.b2 = ReadByte();
                bs.b1 = ReadByte();
            }

            else
            {
                bs.b1 = ReadByte();
                bs.b2 = ReadByte();
                bs.b3 = ReadByte();
                bs.b4 = ReadByte();
                bs.b5 = ReadByte();
                bs.b6 = ReadByte();
                bs.b7 = ReadByte();
                bs.b8 = ReadByte();
            }

            return bs.ul;
        }

        public float ReadFloat()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b4 = ReadByte();
                bs.b3 = ReadByte();
                bs.b2 = ReadByte();
                bs.b1 = ReadByte();
            }

            else
            {
                bs.b1 = ReadByte();
                bs.b2 = ReadByte();
                bs.b3 = ReadByte();
                bs.b4 = ReadByte();
            }

            return bs.f;
        }

        public double ReadDouble()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b8 = ReadByte();
                bs.b7 = ReadByte();
                bs.b6 = ReadByte();
                bs.b5 = ReadByte();
                bs.b4 = ReadByte();
                bs.b3 = ReadByte();
                bs.b2 = ReadByte();
                bs.b1 = ReadByte();
            }

            else
            {
                bs.b1 = ReadByte();
                bs.b2 = ReadByte();
                bs.b3 = ReadByte();
                bs.b4 = ReadByte();
                bs.b5 = ReadByte();
                bs.b6 = ReadByte();
                bs.b7 = ReadByte();
                bs.b8 = ReadByte();
            }

            return bs.d;
        }

        public bool ReadBool()
        {
            return ReadByte() == 1;
        }

        public char ReadChar()
        {
            return Convert.ToChar(ReadByte());
        }

        private void WriteGeneric(IConvertible o)
        {
            TypeCode t = o.GetTypeCode();

            switch (t)
            {
                case TypeCode.Boolean:
                    Write((bool)Convert.ChangeType(o, (typeof(bool))));
                    break;
                case TypeCode.Char:
                    Write((char)Convert.ChangeType(o, (typeof(char))));
                    break;
                case TypeCode.SByte:
                    Write((sbyte)Convert.ChangeType(o, (typeof(sbyte))));
                    break;
                case TypeCode.Byte:
                    Write((byte)Convert.ChangeType(o, (typeof(byte))));
                    break;
                case TypeCode.Int16:
                    Write((short)Convert.ChangeType(o, (typeof(short))));
                    break;
                case TypeCode.UInt16:
                    Write((ushort)Convert.ChangeType(o, (typeof(ushort))));
                    break;
                case TypeCode.Int32:
                    Write((int)Convert.ChangeType(o, (typeof(int))));
                    break;
                case TypeCode.UInt32:
                    Write((uint)Convert.ChangeType(o, (typeof(uint))));
                    break;
                case TypeCode.Int64:
                    Write((long)Convert.ChangeType(o, (typeof(long))));
                    break;
                case TypeCode.UInt64:
                    Write((ulong)Convert.ChangeType(o, (typeof(ulong))));
                    break;
                case TypeCode.Single:
                    Write((float)Convert.ChangeType(o, (typeof(float))));
                    break;
                case TypeCode.Double:
                    Write((double)Convert.ChangeType(o, (typeof(double))));
                    break;
                case TypeCode.String:
                    Write((string)Convert.ChangeType(o, (typeof(string))));
                    break;
            }
        }

        private IConvertible ReadGeneric(Type type)
        {
            TypeCode t = Type.GetTypeCode(type);

            switch (t)
            {
                case TypeCode.Boolean:
                    return ReadBool();
                case TypeCode.Char:
                    return ReadChar();
                case TypeCode.SByte:
                    return ReadSByte();
                case TypeCode.Byte:
                    return ReadByte();
                case TypeCode.Int16:
                    return ReadShort();
                case TypeCode.UInt16:
                    return ReadUShort();
                case TypeCode.Int32:
                    return ReadInt();
                case TypeCode.UInt32:
                    return ReadUInt();
                case TypeCode.Int64:
                    return ReadLong();
                case TypeCode.UInt64:
                    return ReadULong();
                case TypeCode.Single:
                    return ReadFloat();
                case TypeCode.Double:
                    return ReadDouble();
                case TypeCode.String:
                    return ReadString();
                default:
                    return null;
            }
        }

        public void Write(string o)
        {
            Write(Encoding.UTF8.GetBytes(o));
        }

        public void Write(Enum e)
        {
            WriteGeneric(e);
        }

        public void Write(INetworkObject o)
        {
            o.Serialize(this);
        }

        public void Write(IList o)
        {
            ushort length = (ushort)o.Count;
            Write(length);
            Type t;
            Type type = o.GetType();

            if (type.IsGenericType)
            {
                t = type.GetGenericArguments()[0];
            }

            else
            {
                t = type.GetElementType();
            }

            if (typeof(IConvertible).IsAssignableFrom(t))
            {
                for (int i = 0; i < length; i++)
                {
                    WriteGeneric((IConvertible)o[i]);
                }
            }

            if (typeof(IList).IsAssignableFrom(t))
            {
                for (int i = 0; i < length; i++)
                {
                    Write((IList)o[i]);
                }
            }

            if (typeof(INetworkObject).IsAssignableFrom(t))
            {
                for (int i = 0; i < length; i++)
                {
                    Write((INetworkObject)o[i]);
                }
            }
        }

        public string ReadString()
        {
            return Encoding.UTF8.GetString((byte[])ReadList(typeof(byte[])));
        }

        public T ReadEnum<T>() where T : struct, IConvertible
        {
            return (T)Enum.ToObject(typeof(T), ReadGeneric(Enum.GetUnderlyingType(typeof(T))));
        }

        public T ReadNetworkObject<T>() where T : INetworkObject
        {
            return (T)ReadNetworkObject(typeof(T));
        }

        public T ReadList<T>() where T : IList
        {
            return (T)ReadList(typeof(T));
        }

        private INetworkObject ReadNetworkObject(Type type)
        {
            INetworkObject o = (INetworkObject)Activator.CreateInstance(type);
            o.Deserialize(this);
            return o;
        }

        private IList ReadList(Type type)
        {
            Type t;
            IList o;
            ushort length = ReadUShort();

            if (type.IsGenericType)
            {
                t = type.GetGenericArguments()[0];
                o = (IList)Activator.CreateInstance(type);
            }

            else
            {
                t = type.GetElementType();
                o = (IList)Activator.CreateInstance(type, length);
            }

            if (typeof(IConvertible).IsAssignableFrom(t))
            {
                for (int i = 0; i < length; i++)
                {
                    if (type.IsGenericType) o.Add(ReadGeneric(t));
                    else o[i] = ReadGeneric(t);
                }
            }

            if (typeof(IList).IsAssignableFrom(t))
            {
                for (int i = 0; i < length; i++)
                {
                    if (type.IsGenericType) o.Add(ReadList(t));
                    else o[i] = ReadList(t);
                }
            }

            if (typeof(INetworkObject).IsAssignableFrom(t))
            {
                for (int i = 0; i < length; i++)
                {
                    if (type.IsGenericType) o.Add(ReadNetworkObject(t));
                    else o[i] = ReadNetworkObject(t);
                }
            }

            return o;
        }
    }

    public interface INetworkObject
    {
        void Serialize(Packet buffer);
        void Deserialize(Packet buffer);
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ByteStruct
    {
        [FieldOffset(0)]
        public byte b1;
        [FieldOffset(1)]
        public byte b2;
        [FieldOffset(2)]
        public byte b3;
        [FieldOffset(3)]
        public byte b4;
        [FieldOffset(4)]
        public byte b5;
        [FieldOffset(5)]
        public byte b6;
        [FieldOffset(6)]
        public byte b7;
        [FieldOffset(7)]
        public byte b8;

        [FieldOffset(0)]
        public short s;
        [FieldOffset(0)]
        public int i;
        [FieldOffset(0)]
        public long l;
        [FieldOffset(0)]
        public ushort us;
        [FieldOffset(0)]
        public uint ui;
        [FieldOffset(0)]
        public ulong ul;
        [FieldOffset(0)]
        public float f;
        [FieldOffset(0)]
        public double d;
    }
}
