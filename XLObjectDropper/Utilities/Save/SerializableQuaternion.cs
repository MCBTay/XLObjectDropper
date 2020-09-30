using System;
using UnityEngine;

namespace XLObjectDropper.Utilities.Save
{
	/// <summary>
	/// Since unity doesn't flag the Quaternion as serializable, we
	/// need to create our own version. This one will automatically convert
	/// between Quaternion and SerializableQuaternion
	/// source: https://answers.unity.com/questions/956047/serialize-quaternion-or-vector3.html
	/// </summary>
	[System.Serializable]
    public struct SerializableQuaternion
    {
	    public float x;
	    public float y;
	    public float z;
	    public float w;

	    public SerializableQuaternion(Quaternion quaternion)
	    {
		    x = quaternion.x;
		    y = quaternion.y;
		    z = quaternion.z;
		    w = quaternion.w;
	    }

        public SerializableQuaternion(float rX, float rY, float rZ, float rW)
        {
            x = rX;
            y = rY;
            z = rZ;
            w = rW;
        }

        public override string ToString()
        {
            return $"[{x}, {y}, {z}, {w}]";
        }

        public static implicit operator Quaternion(SerializableQuaternion rValue)
        {
            return new Quaternion(rValue.x, rValue.y, rValue.z, rValue.w);
        }

        public static implicit operator SerializableQuaternion(Quaternion rValue)
        {
            return new SerializableQuaternion(rValue.x, rValue.y, rValue.z, rValue.w);
        }
    }
}
