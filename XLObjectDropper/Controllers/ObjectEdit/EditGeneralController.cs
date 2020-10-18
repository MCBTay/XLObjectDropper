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

				var general = SelectedObject?.GetSpawnableFromSpawned()?.Settings?.FirstOrDefault(x => x is EditGeneralController) as EditGeneralController;
				if (general != null)
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
	}
}
