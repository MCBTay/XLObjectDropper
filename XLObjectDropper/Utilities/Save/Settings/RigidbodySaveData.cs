using System;

namespace XLObjectDropper.Utilities.Save.Settings
{
	[Serializable]
	public class RigidbodySaveData : ISettingsSaveData
	{
		public bool enablePhysics;
		public bool enableRespawnRecall;
		public SerializableVector3 recallPosition;
		public SerializableQuaternion recallRotation;
	}
}
