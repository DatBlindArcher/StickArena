﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ArcherNetwork
{
    public partial class NetworkBuffer
    {
        private static ByteStruct bs = new ByteStruct();
        private int index;
        private List<byte> buffer;

        public NetworkBuffer()
        {
            index = 0;
            buffer = new List<byte>();
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
            buffer.Add(o);
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
            return buffer[index++];
        }

        public sbyte ReadSByte()
        {
            return (sbyte)buffer[index++];
        }

        public short ReadShort()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b2 = buffer[index++];
                bs.b1 = buffer[index++];
            }

            else
            {
                bs.b1 = buffer[index++];
                bs.b2 = buffer[index++];
            }

            return bs.s;
        }

        public ushort ReadUShort()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b2 = buffer[index++];
                bs.b1 = buffer[index++];
            }

            else
            {
                bs.b1 = buffer[index++];
                bs.b2 = buffer[index++];
            }

            return bs.us;
        }

        public int ReadInt()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b4 = buffer[index++];
                bs.b3 = buffer[index++];
                bs.b2 = buffer[index++];
                bs.b1 = buffer[index++];
            }

            else
            {
                bs.b1 = buffer[index++];
                bs.b2 = buffer[index++];
                bs.b3 = buffer[index++];
                bs.b4 = buffer[index++];
            }

            return bs.i;
        }

        public uint ReadUInt()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b4 = buffer[index++];
                bs.b3 = buffer[index++];
                bs.b2 = buffer[index++];
                bs.b1 = buffer[index++];
            }

            else
            {
                bs.b1 = buffer[index++];
                bs.b2 = buffer[index++];
                bs.b3 = buffer[index++];
                bs.b4 = buffer[index++];
            }

            return bs.ui;
        }

        public long ReadLong()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b8 = buffer[index++];
                bs.b7 = buffer[index++];
                bs.b6 = buffer[index++];
                bs.b5 = buffer[index++];
                bs.b4 = buffer[index++];
                bs.b3 = buffer[index++];
                bs.b2 = buffer[index++];
                bs.b1 = buffer[index++];
            }

            else
            {
                bs.b1 = buffer[index++];
                bs.b2 = buffer[index++];
                bs.b3 = buffer[index++];
                bs.b4 = buffer[index++];
                bs.b5 = buffer[index++];
                bs.b6 = buffer[index++];
                bs.b7 = buffer[index++];
                bs.b8 = buffer[index++];
            }

            return bs.l;
        }

        public ulong ReadULong()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b8 = buffer[index++];
                bs.b7 = buffer[index++];
                bs.b6 = buffer[index++];
                bs.b5 = buffer[index++];
                bs.b4 = buffer[index++];
                bs.b3 = buffer[index++];
                bs.b2 = buffer[index++];
                bs.b1 = buffer[index++];
            }

            else
            {
                bs.b1 = buffer[index++];
                bs.b2 = buffer[index++];
                bs.b3 = buffer[index++];
                bs.b4 = buffer[index++];
                bs.b5 = buffer[index++];
                bs.b6 = buffer[index++];
                bs.b7 = buffer[index++];
                bs.b8 = buffer[index++];
            }

            return bs.ul;
        }

        public float ReadFloat()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b4 = buffer[index++];
                bs.b3 = buffer[index++];
                bs.b2 = buffer[index++];
                bs.b1 = buffer[index++];
            }

            else
            {
                bs.b1 = buffer[index++];
                bs.b2 = buffer[index++];
                bs.b3 = buffer[index++];
                bs.b4 = buffer[index++];
            }

            return bs.f;
        }

        public double ReadDouble()
        {
            if (BitConverter.IsLittleEndian)
            {
                bs.b8 = buffer[index++];
                bs.b7 = buffer[index++];
                bs.b6 = buffer[index++];
                bs.b5 = buffer[index++];
                bs.b4 = buffer[index++];
                bs.b3 = buffer[index++];
                bs.b2 = buffer[index++];
                bs.b1 = buffer[index++];
            }

            else
            {
                bs.b1 = buffer[index++];
                bs.b2 = buffer[index++];
                bs.b3 = buffer[index++];
                bs.b4 = buffer[index++];
                bs.b5 = buffer[index++];
                bs.b6 = buffer[index++];
                bs.b7 = buffer[index++];
                bs.b8 = buffer[index++];
            }

            return bs.d;
        }

        public bool ReadBool()
        {
            return buffer[index++] == 1;
        }

        public char ReadChar()
        {
            return Convert.ToChar(buffer[index++]);
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