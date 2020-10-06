using UnityEngine;
using XLObjectDropper.UI.Controls.Expandables;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public class EditGeneralController : MonoBehaviour
	{
		public static void AddOptions(GameObject SelectedObject, ObjectEditUI ObjectEdit)
		{
			var generalExpandable = ObjectEdit.AddGeneralSettings();
			var expandable = generalExpandable.GetComponent<GeneralSettingsExpandable>();

			if (expandable == null) return;

			var spawnable = SelectedObject.GetSpawnableFromSpawned() ?? SelectedObject.GetSpawnable();

			expandable.HideInReplays.Toggle.isOn = spawnable.HideInReplays;
			expandable.HideInReplays.onValueChanged += (isOn) => { spawnable.HideInReplays = isOn; };
		}
	}
}
