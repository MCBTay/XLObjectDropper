using System;
using UnityEngine;

namespace XLObjectDropper.Utilities.Save
{
	/// <summary>
	/// Since unity doesn't flag the Vector3 as serializable, we
	/// need to create our own version. This one will automatically convert
	/// between Vector3 and SerializableVector3
	/// source: https://answers.unity.com/questions/956047/serialize-quaternion-or-vector3.html
	/// </summary>
	[Serializable]
	public struct SerializableVector3
	{
		public float x;
		public float y;
		public float z;

		public SerializableVector3(Vector3 vector)
		{
			x = vector.x;
			y = vector.y;
			z = vector.z;
		}

		public SerializableVector3(float rX, float rY, float rZ)
		{
			x = rX;
			y = rY;
			z = rZ;
		}

		public override string ToString()
		{
			return $"[{x}, {y}, {z}]";
		}

		public static implicit operator Vector3(SerializableVector3 rValue)
		{
			return new Vector3(rValue.x, rValue.y, rValue.z);
		}

		public static implicit operator SerializableVector3(Vector3 rValue)
		{
			return new SerializableVector3(rValue.x, rValue.y, rValue.z);
		}
	}
}
