using System;

namespace XLObjectDropper.Utilities.Save.Legacy
{
	[Serializable]
	public class LegacyGameObjectSaveData
	{
		public string objectName;
		public float posX;
		public float posY;
		public float posZ;
		public float rotX;
		public float rotY;
		public float rotZ;
		public float scaleX;
		public float scaleY;
		public float scaleZ;
	}
}
