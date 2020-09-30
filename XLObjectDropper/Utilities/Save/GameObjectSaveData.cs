using System;

namespace XLObjectDropper.Utilities.Save
{
	[Serializable]
	public class GameObjectSaveData
	{
		public string Id;

		public SerializableVector3 position;
		public SerializableQuaternion rotation;
		public SerializableVector3 localScale;

		public LightingSaveData lighting;
		public GrindableSaveData grindables;
	}
}
