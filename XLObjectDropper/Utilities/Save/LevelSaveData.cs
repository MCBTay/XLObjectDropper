using System;
using System.Collections.Generic;

namespace XLObjectDropper.Utilities.Save
{
	[Serializable]
	public class LevelSaveData
	{
		public string levelHash;
		public List<GameObjectSaveData> gameObjects;
	}
}