using System.Collections.Generic;
using UnityEngine;

namespace XLObjectDropper.Utilities
{
	public static class SpawnableManager
	{
		public static List<Spawnable> Prefabs;
		public static List<Spawnable> SpawnedObjects;

		static SpawnableManager()
		{
			Prefabs = new List<Spawnable>();
			SpawnedObjects = new List<Spawnable>();
		}

		public static void DisposeLoadedAssets()
		{
			foreach (var spawnable in Prefabs)
			{
				spawnable.Prefab.SetActive(false);
				Object.Destroy(spawnable.Prefab);
			}

			Prefabs.Clear();
		}

		public static void DeleteSpawnedObjects()
		{
			foreach (var spawnedObject in SpawnedObjects)
			{
				Object.DestroyImmediate(spawnedObject.Prefab);
				Object.DestroyImmediate(spawnedObject.SpawnedInstance);
				Object.DestroyImmediate(spawnedObject.PreviewTexture);
			}

			SpawnedObjects.Clear();
		}
	}
}
