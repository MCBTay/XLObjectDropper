using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;
using XLObjectDropper.Controllers.ObjectEdit;
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

			LevelSaves = new List<LevelSaveData>();
			LevelSaves.AddRange(Utilities.SaveManager.Instance.GetLoadedSavesByLevelHash(LevelManager.Instance.currentLevel.hash));
			LevelSaves.AddRange(Utilities.SaveManager.Instance.GetLoadedLegacySaves(LevelManager.Instance.currentLevel.name));
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

			if (LoadSavedUI.ListContent.transform.childCount > 0)
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

			foreach (var savedGameObject in levelSave.gameObjects)
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

				if (spawnable == null) continue;

				var newGameObject = Instantiate(spawnable.Prefab, position, rotation);
				newGameObject.SetActive(true);

				foreach (var settings in spawnable.Settings)
				{
					settings.ApplySaveSettings(newGameObject, savedGameObject.settings);
				}

				SpawnableManager.SpawnedObjects.Add(new Spawnable(spawnable, newGameObject));
			}

			SaveLoaded?.Invoke();
		}
	}
}
