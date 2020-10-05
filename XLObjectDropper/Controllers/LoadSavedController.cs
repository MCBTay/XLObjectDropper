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

namespace XLObjectDropper.Controllers
{
	public class LoadSavedController : MonoBehaviour
	{
		public static LoadSavedUI LoadSavedUI;
		private List<LevelSaveData> LevelSaves;

		[HideInInspector] public event UnityAction SaveLoaded = () => { };

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
				LoadSavedUI.AddToList(levelSave.levelName ?? "Test", levelSave.dateModified, levelSave.gameObjects.Count,
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
						Utilities.SaveManager.Instance.SaveCurrentSpawnables();
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

			SaveLoaded?.Invoke();
		}
	}
}
