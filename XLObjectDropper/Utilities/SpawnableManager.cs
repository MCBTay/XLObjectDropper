using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLObjectDropper.Controllers.ObjectEdit;
using XLObjectDropper.SpawnableScripts;

namespace XLObjectDropper.Utilities
{
	public static class SpawnableManager
	{
		public static List<Spawnable> LegacyPrefabs;
		public static List<Spawnable> Prefabs;
		public static List<Spawnable> SpawnedObjects;

		static SpawnableManager()
		{
			LegacyPrefabs = new List<Spawnable>();
			Prefabs = new List<Spawnable>();
			SpawnedObjects = new List<Spawnable>();
		}

		public static void AddPrefab(GameObject asset, AssetBundle bundle, bool isLegacy)
		{
			if (isLegacy)
			{
				LegacyPrefabs.Add(new Spawnable(Enumerations.SpawnableType.Other, asset, bundle.name, string.Empty));
				return;
			}

			var type = Enumerations.SpawnableType.Other;
			var menuText = string.Empty;

			var categoryController = asset.GetComponentInChildren<XLCategoryController>(true);
			if (categoryController != null)
			{
				type = categoryController.Type;
				menuText = categoryController.MenuText;
			}

			var styleGroupController = asset.GetComponent<XLStyleGroupController>();
			var styleController = asset.GetComponent<XLStyleController>();

			if (styleController == null && styleGroupController == null)
			{
				Prefabs.Add(new Spawnable(type, asset, bundle.name, menuText));
			}
			else if (styleGroupController != null)
			{
				foreach (var styleObject in styleGroupController.Objects)
				{
					var component = styleObject.GetComponent<XLStyleController>();
					if (component.ShowInObjectSelection)
					{
						var styleCatController = styleObject.GetComponentInChildren<XLCategoryController>(true);
						if (styleCatController != null)
						{
							type = styleCatController.Type;
							menuText = styleCatController.MenuText;
						}

						var altStyles = styleGroupController.Objects.Where(x => !x.GetComponent<XLStyleController>().ShowInObjectSelection).ToList();
						Prefabs.Add(new Spawnable(type, styleObject, bundle.name, menuText, altStyles));
					}
				}
			}
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

			foreach (var spawnable in Prefabs)
			{
				if (name.Equals(spawnable.Prefab.name))
				{
					return spawnable;
				}

				var styleSettings = spawnable.Settings.FirstOrDefault(x => x is EditStyleController) as EditStyleController;
				var style = styleSettings?.Styles.FirstOrDefault(x => name.Equals(x.Prefab.name));

				if (style != null) 
					return style;
			}

			return null;
		}

		public static Spawnable GetSpawnableFromSpawned(this GameObject gameObject)
		{
			var name = gameObject.name.Replace("(Clone)", string.Empty);
			return SpawnedObjects.FirstOrDefault(x => name.StartsWith(x.Prefab.name) && x.SpawnedInstance == gameObject);
		}
		#endregion
	}
}
