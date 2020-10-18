using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

		public static UnityEvent styleSelected;

		public EditStyleController()
		{
			Styles = new List<Spawnable>();
		}

		public void AddOptions(GameObject selectedObject, ObjectEditUI ObjectEdit)
		{
			SelectedObject = selectedObject;

			var spawnable = selectedObject.GetSpawnable();

			var stylesController = spawnable.Settings.FirstOrDefault(x => x is EditStyleController) as EditStyleController;
			if (stylesController?.Styles == null || !stylesController.Styles.Any()) return;

			var newExpandable = ObjectEdit.AddStyleSettings();

			var expandable = newExpandable.GetComponent<Expandable>();
			var styleExpandable = newExpandable.GetComponent<StyleSettingsExpandable>();

			string style = spawnable.Prefab.GetComponent<XLStyleController>().Style;
			string subStyle = spawnable.Prefab.GetComponent<XLStyleController>().SubStyle;

			string name = style;
			if (!string.IsNullOrEmpty(subStyle))
			{
				name += $" / {subStyle}";
			}

			AddListItem(styleExpandable.ListItemPrefab, expandable.PropertiesListContent.transform, spawnable, name);

			foreach (var altStyle in stylesController.Styles)
			{
				style = altStyle.Prefab.GetComponent<XLStyleController>().Style;
				subStyle = altStyle.Prefab.GetComponent<XLStyleController>().SubStyle;

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
				ObjectMovementController.Instance.InstantiateSelectedObject(spawnable);

				ObjectMovementController.Instance.SelectedObject.transform.position = pos;
				ObjectMovementController.Instance.SelectedObject.transform.rotation = rot;

				//Object.DestroyImmediate(SelectedObject);
				//SelectedObject = ObjectMovementController.Instance.SelectedObject;

				ObjectMovementController.Instance.enabled = false;
				styleSelected.Invoke();

			}

			if (updateHighlighted)
			{
				ObjectMovementController.Instance.HighlightedObject = null;
				ObjectMovementController.Instance.HighlightedObjectLayerInfo = null;
			}

			//SelectedObject = Object.Instantiate(spawnable.Prefab, pos, rot);

			//if (parent != null)
			//{
			//	SelectedObject.transform.SetParent(parent);
			//}

			//if (updateSelected)
			//{
			//	ObjectMovementController.Instance.SelectedObject = SelectedObject;
			//	ObjectMovementController.Instance.SelectedObjectLayerInfo = spawnable.Prefab.transform.GetObjectLayers();

			//	ObjectMovementController.Instance.SelectedObject.transform.ChangeLayersRecursively("Ignore Raycast");

			//	UserInterfaceHelper.CustomPassVolume.enabled = true;
			//}
			//if (updateHighlighted)
			//{
			//	ObjectMovementController.Instance.HighlightedObject = SelectedObject;
			//	ObjectMovementController.Instance.HighlightedObjectLayerInfo = spawnable.Prefab.transform.GetObjectLayers();

			//	ObjectMovementController.Instance.HighlightedObject.transform.ChangeLayersRecursively("Ignore Raycast");

			//	UserInterfaceHelper.CustomPassVolume.enabled = true;
			//}
		}

		public ISettingsSaveData ConvertToSaveSettings()
		{
			// Because styles are just different game objects, there's nothing really to save about them.
			return null;
		}
	}
}
