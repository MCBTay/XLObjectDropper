using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLObjectDropper.UI.Controls.Expandables;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities;
using XLObjectDropper.Utilities.Save.Settings;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public class EditGrindablesController : IObjectSettings
	{
		private static EditGrindablesController _instance;
		public static EditGrindablesController Instance => _instance ?? (_instance = new EditGrindablesController());

		public bool GrindableEnabled;
		public bool CopingEnabled;

		public void AddOptions(GameObject SelectedObject, ObjectEditUI ObjectEdit)
		{
			//if (ObjectMovementController.Instance.SelectedObjectLayerInfo == null) return;

			var grindables = SelectedObject.GetPrefab().GetChildrenOnLayer("Grindable");
			bool hasGrindables = grindables != null && grindables.Any();

			var copings = SelectedObject.GetPrefab().GetChildrenOnLayer("Coping");
			bool hasCoping = copings != null && copings.Any();

			if (!hasGrindables && !hasCoping) return;

			var grindableExpandable = ObjectEdit.AddGrindableSettings();

			var expandable = grindableExpandable.GetComponent<GrindableSettingsExpandable>();

			var spawnable = SelectedObject.GetSpawnableFromSpawned() ?? SelectedObject.GetSpawnable();
			if (spawnable == null) return;

			expandable.GrindablesToggle.Toggle.interactable = hasGrindables;
			if (hasGrindables)
			{
				expandable.GrindablesToggle.Toggle.isOn = GrindableEnabled;

				expandable.GrindablesToggle.Toggle.interactable = true;
				expandable.GrindablesToggle.Toggle.onValueChanged.AddListener((isOn) =>
				{
					GrindableEnabled = isOn;
					DisableLayer(spawnable.PrefabLayerInfo, "Grindable", isOn);
				});
			}

			expandable.CopingToggle.Toggle.interactable = hasCoping;
			if (hasCoping)
			{
				expandable.CopingToggle.Toggle.isOn = CopingEnabled;

				expandable.CopingToggle.Toggle.interactable = true;
				expandable.CopingToggle.Toggle.onValueChanged.AddListener((isOn) =>
				{
					CopingEnabled = isOn;
					DisableLayer(spawnable.PrefabLayerInfo, "Coping", isOn);
				});
			}
		}

		private void DisableLayer(LayerInfo layerInfo, string layerName, bool isEnabled)
		{
			if (layerInfo.Layer == LayerMask.NameToLayer(layerName))
			{
				layerInfo.Enabled = isEnabled;
			}

			foreach (var child in layerInfo.Children)
			{
				DisableLayer(child, layerName, isEnabled);
			}
		}

		public ISettingsSaveData ConvertToSaveSettings()
		{
			return new GrindableSaveData
			{
				copingEnabled = CopingEnabled,
				grindablesEnabled = GrindableEnabled
			};
		}

		public void ApplySaveSettings(GameObject selectedObject, List<ISettingsSaveData> settings)
		{
			if (settings == null || !settings.Any()) return;

			var grindableSettings = settings.OfType<GrindableSaveData>().ToList();
			if (!grindableSettings.Any()) return;

			var grindableSetting = grindableSettings.First();

			GrindableEnabled = grindableSetting.grindablesEnabled;
			CopingEnabled = grindableSetting.copingEnabled;

			var spawnable = selectedObject.GetSpawnableFromSpawned() ?? selectedObject.GetSpawnable();
			if (spawnable == null) return;

			DisableLayer(spawnable.PrefabLayerInfo, "Grindable", GrindableEnabled);
			DisableLayer(spawnable.PrefabLayerInfo, "Coping", CopingEnabled);
		}
	}
}
