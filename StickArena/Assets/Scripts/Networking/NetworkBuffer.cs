using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ArcherNetwork
{
    public partial class NetworkBuffer
    {
        private static ByteStruct bs = new ByteStruct();
        private List<byte> writeBuffer;
        private int index;
        private byte[] readBuffer;

        public NetworkBuffer()
        {
            writeBuffer = new List<byte>();
        }

        public NetworkBuffer(byte[] buffer)
        {
            index = 0;
            readBuffer = buffer;
        }

        public byte[] getBytes()
        {
            return writeBuffer.ToArray();
        }

        public void ForceIndex(int index, Action action)
        {
            int temp = this.index;
            this.index = index;
            action();
            this.index = temp;
        }
        
        public void Write(byte o)
        {
            writeBuffer.Add(o);
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
            return readBuffer[index++];
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