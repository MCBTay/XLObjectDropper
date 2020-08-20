using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace XLObjectDropper
{
	public class SaveManager
	{
		private static SaveManager __instance;
		public static SaveManager Instance => __instance ?? (__instance = new SaveManager());

		public void SaveCurrentSpawnables()
		{
			var itemsToSave = new List<GameObjectSaveData>();
			var spawnedItems = ObjectMovementController.Instance.SpawnedObjects;

			if (spawnedItems == null || !spawnedItems.Any()) return;

			foreach (var spawnable in spawnedItems)
			{
				itemsToSave.Add(new GameObjectSaveData
				{
					Id = 1,
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

			string json = JsonConvert.SerializeObject(itemsToSave);
			File.WriteAllText(Path.Combine(Main.ModPath, "test.json"), json);
		}

		public void LoadSpawnables()
		{
			var filePath = Path.Combine(Main.ModPath, "test.json");

			if (!File.Exists(filePath)) return;

			var spawnables = JsonConvert.DeserializeObject<List<GameObjectSaveData>>(File.ReadAllText(filePath));
			if (spawnables == null || !spawnables.Any()) return;

			foreach (var spawnable in spawnables)
			{
				var position = new Vector3(spawnable.positionX, spawnable.positionY, spawnable.positionZ);
				var rotation = new Quaternion(spawnable.rotationX, spawnable.rotationY, spawnable.rotationZ, spawnable.rotationW);

				var newObject = Object.Instantiate(ObjectMovementController.Instance.PreviewObject, position, rotation);
				newObject.SetActive(true);

				newObject.transform.ChangeLayersRecursively("Default");

				ObjectMovementController.Instance.SpawnedObjects.Add(newObject);
			}
		}
	}

	[System.Serializable]
	public class GameObjectSaveData
	{
		public int Id;
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
