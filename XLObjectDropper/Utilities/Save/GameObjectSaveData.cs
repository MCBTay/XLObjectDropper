using System;
using System.Collections.Generic;
using XLObjectDropper.Utilities.Save.Settings;

namespace XLObjectDropper.Utilities.Save
{
	[Serializable]
	public class GameObjectSaveData
	{
		public string Id;

		public SerializableVector3 position;
		public SerializableQuaternion rotation;
		public SerializableVector3 localScale;

		public List<ISettingsSaveData> settings;

		public GameObjectSaveData()
		{
			settings = new List<ISettingsSaveData>();
		}
	}
}
