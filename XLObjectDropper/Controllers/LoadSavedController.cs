using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities;
using XLObjectDropper.Utilities.Save;
using XLObjectDropper.Utilities.Save.Settings;
using Object = UnityEngine.Object;

namespace XLObjectDropper.Controllers
{
	public class LoadSavedController : MonoBehaviour
	{
		public static LoadSavedUI LoadSavedUI;
		private List<LevelSaveData> LevelSaves;

		public event UnityAction SaveLoaded = () => { };

		public void Awake()
		{
			Utilities.SaveManager.Instance.LoadAllSaves();

			LevelSaves = Utilities.SaveManager.Instance.GetLoadedSavesByLevelHash(LevelManager.Instance.currentLevel.hash);
		}

		public void Start()
		{
			LoadSavedUI.ClearList();

			foreach (var levelSave in LevelSaves)
			{
				LoadSavedUI.AddToList(levelSave.fileName ?? levelSave.fileName, levelSave.dateModified, levelSave.gameObjects.Count,
					() => ObjectClicked(levelSave),
					() => { UISounds.Instance?.PlayOneShotSelectionChange(); });
			}

			EventSystem.current.SetSelectedGameObject(LoadSavedUI.ListContent.transform.GetChild(0).gameObject);
		}


		private void SetItemsInteractable(bool interactable)
		{
			for (var i = LoadSavedUI.ListContent.transform.childCount - 1; i >= 0; i--)
			{
				var objectA = LoadSavedUI.ListContent.transform.GetChild(i);
				objectA.gameObject.GetComponent<Button>().interactable = interactable;
			}
		}

		private void ObjectClicked(LevelSaveData levelSave)
		{
			UISounds.Instance?.PlayOneShotSelectMajor();

			if (SpawnableManager.SpawnedObjects != null && SpawnableManager.SpawnedObjects.Any())
			{
				if (Utilities.SaveManager.Instance.HasUnsavedChanges)
				{
					SetItemsInteractable(false);

					var test = LoadSavedUI.CreateUnsavedChangesDialog();

					test.YesClicked += () =>
					{
						UISounds.Instance?.PlayOneShotSelectMajor();
						//TODO: launch save here
						Utilities.SaveManager.Instance.SaveCurrentSpawnables($"{LevelManager.Instance.currentLevel.name}_{DateTime.Now:MM.dd.yyyyThh.mm.ss}");
						LoadSave(levelSave);
					};

					test.NoClicked += () =>
					{
						UISounds.Instance?.PlayOneShotSelectMajor();
						LoadSave(levelSave);
					};
				}
				else
				{
					LoadSave(levelSave);
				}
			}
			else
			{
				LoadSave(levelSave);
			}
		}

		private void LoadSave(LevelSaveData levelSave)
		{
			if (string.IsNullOrEmpty(levelSave.filePath) || !File.Exists(levelSave.filePath)) return;
			if (levelSave.gameObjects == null || !levelSave.gameObjects.Any()) return;

			SpawnableManager.DeleteSpawnedObjects();

			foreach (var spawnable in levelSave.gameObjects)
			{
				var position = new Vector3(spawnable.position.x, spawnable.position.y, spawnable.position.z);
				var rotation = new Quaternion(spawnable.rotation.x, spawnable.rotation.y, spawnable.rotation.z, spawnable.rotation.w);

				var prefab = SpawnableManager.Prefabs.FirstOrDefault(x => spawnable.Id.StartsWith(x.Prefab.name));

				if (prefab == null) continue;

				var newObject = Object.Instantiate(prefab.Prefab, position, rotation);
				newObject.SetActive(true);

				if (spawnable.settings.FirstOrDefault(x => x is LightingSaveData) != null)
				{
					var lightingSettings = spawnable.settings.FirstOrDefault(x => x is LightingSaveData) as LightingSaveData;

					var light = newObject.GetComponentInChildren<Light>(true);
					var hdLight = light.GetComponent<HDAdditionalLightData>();

					light.enabled = lightingSettings.enabled;
					hdLight.enabled = lightingSettings.enabled;

					hdLight.lightUnit = lightingSettings.unit;
					hdLight.intensity = lightingSettings.intensity;
					hdLight.range = lightingSettings.range;
					hdLight.SetSpotAngle(lightingSettings.angle);
					hdLight.color = new Color(lightingSettings.color.x, lightingSettings.color.y, lightingSettings.color.z);
				}

				//TODO: Load general and grindable settings too

				SpawnableManager.SpawnedObjects.Add(new Spawnable(prefab, newObject));
			}

			SaveLoaded?.Invoke();
		}
	}
}
