using System;
using System.Collections.Generic;

namespace XLObjectDropper.Utilities.Save
{
	[Serializable]
	public class LevelSaveData
	{
		public string filePath;
		public string saveName;
		public string levelHash;
		public string levelName;
		public DateTime dateModified;
		public List<GameObjectSaveData> gameObjects;

		public LevelSaveData()
		{
			filePath = string.Empty;
			saveName = string.Empty;
			levelHash = string.Empty;
			levelName = string.Empty;
			dateModified = DateTime.MinValue;
			gameObjects = new List<GameObjectSaveData>();
		}
	}
}