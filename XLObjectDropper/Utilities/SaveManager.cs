using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityModManagerNet;
using XLObjectDropper.Utilities.Save;
using Object = UnityEngine.Object;

namespace XLObjectDropper.Utilities
{
	public class SaveManager
	{
		private static SaveManager __instance;
		public static SaveManager Instance => __instance ?? (__instance = new SaveManager());

		public UnityModManager.ModEntry ModEntry;

		public static string SaveDir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SkaterXL", "XLObjectDropper", "Saves");

		public List<LevelSaveData> LoadedSaves;

		public void SaveCurrentSpawnables()
		{
			var levelConfigToSave = new LevelSaveData
			{ 
				levelHash = LevelManager.Instance.currentLevel.hash, 
				levelName = LevelManager.Instance.currentLevel.name,
				dateModified = DateTime.Now,
			};

			var spawnedItems = SpawnableManager.SpawnedObjects;

			if (spawnedItems == null || !spawnedItems.Any()) return;

			foreach (var spawnable in spawnedItems)
			{
				var instance = spawnable.SpawnedInstance;

				var objectSaveData = new GameObjectSaveData
				{
					Id = instance.name,
					position = new SerializableVector3(instance.transform.position),
					rotation = new SerializableQuaternion(instance.transform.rotation),
					localScale = new SerializableVector3(instance.transform.localScale)
				};

				var light = instance.GetComponentInChildren<Light>(true);

				if (light != null)
				{
					var hdLight = light.GetComponent<HDAdditionalLightData>();

					objectSaveData.lighting = new LightingSaveData
					{
						intensity = hdLight.intensity,
						unit = hdLight.lightUnit,
						angle = light.spotAngle,
						range = hdLight.range,
						enabled = hdLight.enabled,
						color = new SerializableVector3(hdLight.color.r, hdLight.color.g, hdLight.color.b)
					};
				}

				levelConfigToSave.gameObjects.Add(objectSaveData);
			}

			string json = JsonConvert.SerializeObject(levelConfigToSave);

			var currentSaveDir = Path.Combine(SaveDir, levelConfigToSave.levelName);

			if (!Directory.Exists(currentSaveDir))
			{
				Directory.CreateDirectory(currentSaveDir);
			}

			File.WriteAllText(Path.Combine(currentSaveDir, $"{levelConfigToSave.levelName}_{DateTime.Now:MM.dd.yyyyThh.mm.ss}.json"), json);
		}

		public void LoadAllSaves()
		{
			if (!Directory.Exists(SaveDir))
			{
				Directory.CreateDirectory(SaveDir);
				return;
			}

			if (LoadedSaves == null)
			{
				LoadedSaves = new List<LevelSaveData>();
			}
			else
			{
				LoadedSaves.Clear();
			}

			var saveFiles = Directory.GetFiles(SaveDir, "*.json", SearchOption.AllDirectories);

			if (saveFiles.Any())
			{
				foreach (var saveFile in saveFiles)
				{
					var content = File.ReadAllText(saveFile);

					if (string.IsNullOrEmpty(content))
						continue;

					try
					{
						LoadedSaves.Add(JsonConvert.DeserializeObject<LevelSaveData>(content));
					}
					catch (Exception ex)
					{
						UnityModManager.Logger.Log($"XLObjectDropper: Unable to deserialize {saveFile}");
						continue;
					}
				}
			}
		}

		public List<LevelSaveData> GetLoadedSavesByLevelHash(string hash)
		{
			if (LoadedSaves == null || !LoadedSaves.Any())
			{
				return new List<LevelSaveData>();
			}

			return LoadedSaves.Where(x => x.levelHash == hash).ToList();
		}

		public List<LevelSaveData> GetLoadedSavesByLevelName(string name)
		{
			if (LoadedSaves == null || !LoadedSaves.Any())
			{
				return new List<LevelSaveData>();
			}

			return LoadedSaves.Where(x => x.levelName == name).ToList();
		}

		public void LoadSave()
		{
			var filePath = Path.Combine(SaveDir, "test.json");

			if (!Directory.Exists(SaveDir))
			{
				Directory.CreateDirectory(SaveDir);
			}

			if (!File.Exists(filePath)) return;

			var levelSaveData = JsonConvert.DeserializeObject<LevelSaveData>(File.ReadAllText(filePath));
			if (levelSaveData.gameObjects == null || !levelSaveData.gameObjects.Any()) return;

			foreach (var spawnable in levelSaveData.gameObjects)
			{
				var position = new Vector3(spawnable.position.x, spawnable.position.y, spawnable.position.z);
				var rotation = new Quaternion(spawnable.rotation.x, spawnable.rotation.y, spawnable.rotation.z, spawnable.rotation.w);

				var prefab = SpawnableManager.Prefabs.FirstOrDefault(x => spawnable.Id.StartsWith(x.Prefab.name));

				if (prefab == null) continue;

				var newObject = Object.Instantiate(prefab.Prefab, position, rotation);
				newObject.SetActive(true);

				if (spawnable.lighting != null)
				{
					var light = newObject.GetComponentInChildren<Light>(true);
					var hdLight = light.GetComponent<HDAdditionalLightData>();

					light.enabled = spawnable.lighting.enabled;
					hdLight.enabled = spawnable.lighting.enabled;

					hdLight.lightUnit = spawnable.lighting.unit;
					hdLight.intensity = spawnable.lighting.intensity;
					hdLight.range = spawnable.lighting.range;
					hdLight.SetSpotAngle(spawnable.lighting.angle);
					hdLight.color = new Color(spawnable.lighting.color.x, spawnable.lighting.color.y, spawnable.lighting.color.z);
				}

				SpawnableManager.SpawnedObjects.Add(new Spawnable(prefab.Prefab, newObject, prefab.PreviewTexture));
			}
		}

		// TODO: Does this really belong here?
		public void SaveSettings()
		{
			Settings.Instance.Save(ModEntry);
		}
	}
}
