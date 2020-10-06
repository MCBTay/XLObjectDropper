using System.Collections.Generic;
using System.Linq;
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
				Object.DestroyImmediate(spawnedObject.SpawnedInstance);
			}

			SpawnedObjects.Clear();
		}

		#region Extensions
		public static GameObject GetPrefab(this GameObject gameObject)
		{
			return gameObject.GetSpawnable()?.Prefab;
		}

		public static Spawnable GetSpawnable(this GameObject gameObject)
		{
			var name = gameObject.name.Replace("(Clone)", string.Empty);
			return Prefabs.FirstOrDefault(x => name.Equals(x.Prefab.name));
		}

		public static Spawnable GetSpawnableFromSpawned(this GameObject gameObject)
		{
			var name = gameObject.name.Replace("(Clone)", string.Empty);
			return SpawnedObjects.FirstOrDefault(x => name.Equals(x.Prefab.name));
		}
		#endregion
	}
}
