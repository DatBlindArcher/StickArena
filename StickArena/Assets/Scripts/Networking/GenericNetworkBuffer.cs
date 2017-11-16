using System;
using System.Text;
using System.Collections;

namespace ArcherNetwork
{
    public partial class NetworkBuffer
    {
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

        public Enum ReadEnum(Type type)
        {
            return (Enum) Enum.ToObject(type, ReadGeneric(Enum.GetUnderlyingType(type)));
        }

        public INetworkObject ReadNetworkObject(Type type)
        {
            INetworkObject o = (INetworkObject)Activator.CreateInstance(type);
            o.Deserialize(this);
            return o;
        }

        public IList ReadList(Type type)
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
        void Serialize(NetworkBuffer buffer);
        void Deserialize(NetworkBuffer buffer);
    }
}