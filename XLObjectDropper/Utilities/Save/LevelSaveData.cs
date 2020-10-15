using System;
using System.Collections.Generic;

namespace XLObjectDropper.Utilities.Save
{
	[Serializable]
	public class LevelSaveData
	{
		[NonSerialized] public string filePath;
		[NonSerialized] public string fileName;
		[NonSerialized] public bool isLegacy;
		public string levelHash;
		public string levelName;
		public DateTime dateModified;
		public List<GameObjectSaveData> gameObjects;

		public LevelSaveData()
		{
			filePath = string.Empty;
			fileName = string.Empty;
			levelHash = string.Empty;
			levelName = string.Empty;
			dateModified = DateTime.MinValue;
			gameObjects = new List<GameObjectSaveData>();
		}
	}
}