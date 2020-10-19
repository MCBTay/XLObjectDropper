using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLObjectDropper.UI.Controls.Expandables;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities;
using XLObjectDropper.Utilities.Save.Settings;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public class EditGeneralController : IObjectSettings
	{
		private static EditGeneralController _instance;
		public static EditGeneralController Instance => _instance ?? (_instance = new EditGeneralController());

		public bool HideInReplays;

		public void AddOptions(GameObject SelectedObject, ObjectEditUI ObjectEdit)
		{
			var generalExpandable = ObjectEdit.AddGeneralSettings();
			var expandable = generalExpandable.GetComponent<GeneralSettingsExpandable>();

			if (expandable == null) return;

			expandable.HideInReplays.Toggle.isOn = HideInReplays;
			expandable.HideInReplays.Toggle.onValueChanged.AddListener((isOn) =>
			{
				HideInReplays = isOn;

				if (SelectedObject?.GetSpawnableFromSpawned()?.Settings?.FirstOrDefault(x => x is EditGeneralController) is EditGeneralController general)
				{
					general.HideInReplays = HideInReplays;
				}
			});
		}

		public ISettingsSaveData ConvertToSaveSettings()
		{
			return new GeneralSaveData
			{
				hideInReplays = HideInReplays
			};
		}

		public void ApplySaveSettings(GameObject selectedObject, List<ISettingsSaveData> settings)
		{
			if (settings == null || !settings.Any()) return;

			var generalSettings = settings.OfType<GeneralSaveData>().ToList();
			if (!generalSettings.Any()) return;

			var generalSetting = generalSettings.First();

			HideInReplays = generalSetting.hideInReplays;
		}
	}
}
