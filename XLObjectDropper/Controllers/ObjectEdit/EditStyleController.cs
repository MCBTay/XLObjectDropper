using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XLObjectDropper.SpawnableScripts;
using XLObjectDropper.UI.Controls.Expandables;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities;
using XLObjectDropper.Utilities.Save.Settings;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public class EditStyleController : IObjectSettings
	{
		private static EditStyleController _instance;
		public static EditStyleController Instance => _instance ?? (_instance = new EditStyleController());

		public GameObject SelectedObject;
		public List<Spawnable> Styles;

		public EditStyleController()
		{
			Styles = new List<Spawnable>();
		}

		public void AddOptions(GameObject SelectedObject, ObjectEditUI ObjectEdit)
		{
			this.SelectedObject = SelectedObject;

			var spawnable = SelectedObject.GetSpawnable();

			var editStyleController = spawnable.Settings.FirstOrDefault(x => x is EditStyleController) as EditStyleController;
			if (editStyleController?.Styles == null || !editStyleController.Styles.Any()) return;

			var newExpandable = ObjectEdit.AddStyleSettings();

			var expandable = newExpandable.GetComponent<Expandable>();
			var styleExpandable = newExpandable.GetComponent<StyleSettingsExpandable>();

			var styleController = spawnable.Prefab.GetComponent<XLStyleController>();
			string style = styleController?.Style;
			string subStyle = styleController?.SubStyle;

			string name = style;
			if (!string.IsNullOrEmpty(subStyle))
			{
				name += $" / {subStyle}";
			}

			AddListItem(styleExpandable.ListItemPrefab, expandable.PropertiesListContent.transform, spawnable, name);

			foreach (var altStyle in editStyleController.Styles.Where(x => x != spawnable))
			{
				styleController = altStyle.Prefab.GetComponent<XLStyleController>();
				style = styleController?.Style;
				subStyle = styleController?.SubStyle;

				name = style;
				if (!string.IsNullOrEmpty(subStyle))
				{
					name += $" / {subStyle}";
				}

				AddListItem(styleExpandable.ListItemPrefab, expandable.PropertiesListContent.transform, altStyle, name);
			}
		}

		private void AddListItem(GameObject prefab, Transform listContent, Spawnable spawnable, string customName = null)
		{
			var listItem = Object.Instantiate(prefab, listContent);

			if (!string.IsNullOrEmpty(customName))
			{
				listItem.GetComponentInChildren<TMP_Text>().SetText(customName);
			}
			else
			{
				listItem.GetComponentInChildren<TMP_Text>().SetText(spawnable.Prefab.name.Replace('_', ' '));
			}

			listItem.GetComponent<Button>().onClick.AddListener(() => StyleClicked(spawnable));

			//if (objectSelected != null)
			//{
			//	listItem.GetComponent<ObjectSelectionListItem>().onSelect.AddListener(objectSelected);
			//}

			listItem.GetComponentInChildren<RawImage>().texture = spawnable.PreviewTexture;

			listItem.SetActive(true);
		}

		private void StyleClicked(Spawnable spawnable)
		{
			var pos = SelectedObject.transform.position;
			var rot = SelectedObject.transform.rotation;
			var parent = SelectedObject.transform.parent;

			var updateSelected = SelectedObject == ObjectMovementController.Instance.SelectedObject;
			var updateHighlighted = SelectedObject == ObjectMovementController.Instance.HighlightedObject;

			if (updateSelected)
			{
				ObjectMovementController.Instance.enabled = true;

				var spawnedObject = ObjectMovementController.Instance.SelectedObject.GetSpawnableFromSpawned();
				if (spawnedObject != null)
				{
					SpawnableManager.SpawnedObjects.Remove(spawnedObject);
					ObjectMovementController.Instance.ExistingObject = false;
				}

				ObjectMovementController.Instance.InstantiateSelectedObject(spawnable);
				SelectedObject = ObjectMovementController.Instance.SelectedObject;
				ObjectMovementController.Instance.SelectedObject.transform.position = pos;
				ObjectMovementController.Instance.SelectedObject.transform.rotation = rot;

				ObjectMovementController.Instance.enabled = false;
			}

			if (updateHighlighted)
			{
				var spawnedObject = ObjectMovementController.Instance.HighlightedObject.GetSpawnableFromSpawned();
				SpawnableManager.SpawnedObjects.Remove(spawnedObject);

				ObjectMovementController.Instance.enabled = true;

				Object.DestroyImmediate(ObjectMovementController.Instance.HighlightedObject);
				ObjectMovementController.Instance.HighlightedObject = null;

				ObjectMovementController.Instance.HighlightedObject = Object.Instantiate(spawnable.Prefab);

				ObjectMovementController.Instance.HighlightedObject.transform.position = pos;
				ObjectMovementController.Instance.HighlightedObject.transform.rotation = rot;

				SpawnableManager.SpawnedObjects.Add(new Spawnable(spawnable, ObjectMovementController.Instance.HighlightedObject));

				SelectedObject = ObjectMovementController.Instance.HighlightedObject;
				//UserInterfaceHelper.CustomPassVolume.enabled = true;

				ObjectMovementController.Instance.enabled = false;
			}
		}

		public ISettingsSaveData ConvertToSaveSettings()
		{
			// Because styles are just different game objects, there's nothing really to save about them.
			return null;
		}

		public void ApplySaveSettings(GameObject selectedobject, List<ISettingsSaveData> settings)
		{
			// Because styles are just different game objects, there's nothing really to save about them.
			return;
		}
	}
}
