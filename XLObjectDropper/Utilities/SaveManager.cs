using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;
using XLObjectDropper.Controllers;
using Object = UnityEngine.Object;

namespace XLObjectDropper.Utilities
{
	public class SaveManager
	{
		private static SaveManager __instance;
		public static SaveManager Instance => __instance ?? (__instance = new SaveManager());

		public UnityModManager.ModEntry ModEntry { get; set; }

		public void SaveCurrentSpawnables()
		{
			var levelConfigToSave = new LevelSaveData { levelHash = LevelManager.Instance.currentLevel.hash };
			levelConfigToSave.gameObjects = new List<GameObjectSaveData>();
			
			var spawnedItems = ObjectDropperController.SpawnedObjects;

			if (spawnedItems == null || !spawnedItems.Any()) return;

			foreach (var spawnable in spawnedItems)
			{
				levelConfigToSave.gameObjects.Add(new GameObjectSaveData
				{
					Id = spawnable.SpawnedInstance.name,
					positionX = spawnable.SpawnedInstance.transform.position.x,
					positionY = spawnable.SpawnedInstance.transform.position.y,
					positionZ = spawnable.SpawnedInstance.transform.position.z,
					rotationX = spawnable.SpawnedInstance.transform.rotation.x,
					rotationY = spawnable.SpawnedInstance.transform.rotation.y,
					rotationZ = spawnable.SpawnedInstance.transform.rotation.z,
					rotationW = spawnable.SpawnedInstance.transform.rotation.w,
					scaleX = spawnable.SpawnedInstance.transform.localScale.x,
					scaleY = spawnable.SpawnedInstance.transform.localScale.y,
					scaleZ = spawnable.SpawnedInstance.transform.localScale.z,
				});
			}

			string json = JsonConvert.SerializeObject(levelConfigToSave);
			File.WriteAllText(Path.Combine(Main.ModPath, "test.json"), json);
		}

		public void LoadSpawnables()
		{
			var filePath = Path.Combine(Main.ModPath, "test.json");

			if (!File.Exists(filePath)) return;

			var levelSaveData = JsonConvert.DeserializeObject<LevelSaveData>(File.ReadAllText(filePath));
			if (levelSaveData.gameObjects == null || !levelSaveData.gameObjects.Any()) return;

			foreach (var spawnable in levelSaveData.gameObjects)
			{
				var position = new Vector3(spawnable.positionX, spawnable.positionY, spawnable.positionZ);
				var rotation = new Quaternion(spawnable.rotationX, spawnable.rotationY, spawnable.rotationZ, spawnable.rotationW);

				var prefab = AssetBundleHelper.LoadedSpawnables.FirstOrDefault(x => spawnable.Id.StartsWith(x.Prefab.name));

				if (prefab == null) continue;

				var newObject = Object.Instantiate(prefab.Prefab, position, rotation);
				newObject.SetActive(true);

				newObject.transform.ChangeLayersRecursively("Default");

				ObjectDropperController.SpawnedObjects.Add(new Spawnable(prefab.Prefab, newObject, prefab.PreviewTexture));
			}
		}

		public void SaveSettings()
		{
			Settings.Instance.Save(ModEntry);
		}
	}

	[System.Serializable]
	public class LevelSaveData
	{
		public string levelHash;
		public List<GameObjectSaveData> gameObjects;
	}

	[System.Serializable]
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
	}
}
