using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;
using XLObjectDropper.Controllers;
using Object = UnityEngine.Object;

namespace XLObjectDropper
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
			
			var spawnedItems = ObjectMovementController.Instance.SpawnedObjects;

			if (spawnedItems == null || !spawnedItems.Any()) return;

			foreach (var spawnable in spawnedItems)
			{
				levelConfigToSave.gameObjects.Add(new GameObjectSaveData
				{
					Id = spawnable.name,
					positionX = spawnable.transform.position.x,
					positionY = spawnable.transform.position.y,
					positionZ = spawnable.transform.position.z,
					rotationX = spawnable.transform.rotation.x,
					rotationY = spawnable.transform.rotation.y,
					rotationZ = spawnable.transform.rotation.z,
					rotationW = spawnable.transform.rotation.w,
					scaleX = spawnable.transform.localScale.x,
					scaleY = spawnable.transform.localScale.y,
					scaleZ = spawnable.transform.localScale.z,
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

				ObjectMovementController.Instance.SpawnedObjects.Add(newObject);
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
