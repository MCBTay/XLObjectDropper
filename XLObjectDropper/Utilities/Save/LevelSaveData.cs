using System;
using System.Collections.Generic;

namespace XLObjectDropper.Utilities.Save
{
	[Serializable]
	public class LevelSaveData
	{
		public string levelHash;
		public string levelName;
		public DateTime dateModified;
		public List<GameObjectSaveData> gameObjects;

		public LevelSaveData()
		{
			levelHash = string.Empty;
			levelName = string.Empty;
			dateModified = DateTime.MinValue;
			gameObjects = new List<GameObjectSaveData>();
		}
	}
}