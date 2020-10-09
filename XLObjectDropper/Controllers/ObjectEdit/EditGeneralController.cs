using UnityEngine;
using XLObjectDropper.UI.Controls.Expandables;
using XLObjectDropper.UI.Menus;

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
			expandable.HideInReplays.Toggle.onValueChanged.AddListener((isOn) => { HideInReplays = isOn; });
		}
	}
}
