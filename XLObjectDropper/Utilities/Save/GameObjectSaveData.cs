using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLObjectDropper.Controllers.ObjectEdit;
using XLObjectDropper.Utilities.Save.Settings;

namespace XLObjectDropper.Utilities.Save
{
	[Serializable]
	public class GameObjectSaveData
	{
		public string Id;
		public string bundleName;
		public SerializableVector3 position;
		public SerializableQuaternion rotation;
		public SerializableVector3 localScale;

		public List<ISettingsSaveData> settings;

		public GameObjectSaveData()
		{
			settings = new List<ISettingsSaveData>();
		}
	}

	public static class GameObjectSaveDataExtensions
	{
		public static void Instantiate(this GameObjectSaveData savedGameObject)
		{
			var position = new Vector3(savedGameObject.position.x, savedGameObject.position.y, savedGameObject.position.z);
			var rotation = new Quaternion(savedGameObject.rotation.x, savedGameObject.rotation.y, savedGameObject.rotation.z, savedGameObject.rotation.w);

			Spawnable spawnable = null;

			foreach (var spawnablePrefab in SpawnableManager.Prefabs)
			{
				var savedObjName = savedGameObject.Id.Replace("(Clone)", string.Empty).Trim();

				if (savedObjName.Equals(spawnablePrefab.Prefab.name))
				{
					spawnable = spawnablePrefab;
					break;
				}

				var styleSettings = spawnablePrefab.Settings.FirstOrDefault(x => x is EditStyleController) as EditStyleController;
				var style = styleSettings?.Styles.FirstOrDefault(x => savedObjName.Equals(x.Prefab.name));

				if (style != null)
				{
					spawnable = style;
					break;
				}
			}

			if (spawnable == null) return;

			var newGameObject = GameObject.Instantiate(spawnable.Prefab, position, rotation);
			newGameObject.SetActive(true);

			var newSpawnable = new Spawnable(spawnable, newGameObject);
			SpawnableManager.SpawnedObjects.Add(newSpawnable);

			foreach (var settings in newSpawnable.Settings)
			{
				settings.ApplySaveSettings(newGameObject, savedGameObject.settings);
			}
		}
	}
}
