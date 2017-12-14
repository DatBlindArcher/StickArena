using UnityEngine;

namespace ArcherNetwork
{
    public partial class Packet
    {
        public void Write(Vector2 o)
        {
            Write(o.x);
            Write(o.y);
        }

        public void Write(Vector3 o)
        {
            Write(o.x);
            Write(o.y);
            Write(o.z);
        }

        public void Write(Vector4 o)
        {
            Write(o.x);
            Write(o.y);
            Write(o.z);
            Write(o.w);
        }

        public void Write(Quaternion o)
        {
            Write(o.x);
            Write(o.y);
            Write(o.z);
            Write(o.w);
        }

        public Vector2 ReadVector2()
        {
            return new Vector2(ReadFloat(), ReadFloat());
        }

        public Vector3 ReadVector3()
        {
            return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
        }

        public Vector4 ReadVector4()
        {
            return new Vector4(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
        }

        public Quaternion ReadQuaternion()
        {
            return new Quaternion(ReadFloat(), ReadFloat(), ReadFloat(), ReadFloat());
        }
    }
}