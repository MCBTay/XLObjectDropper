using System;

namespace XLObjectDropper.Utilities.Save
{
	[Serializable]
	public class GameObjectSaveData
	{
		public string Id;
		public float positionX;
		public float positionY;
		public float positionZ;
		public float rotationX;
		public float rotationY;
		public float rotationZ;
		public float rotationW;
		public float scaleX;
		public float scaleY;
		public float scaleZ;

		public LightingSaveData lighting;
		public GrindableSaveData grindables;
	}
}
