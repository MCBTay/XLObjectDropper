using UnityEngine;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities.Save.Settings;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public interface IObjectSettings
	{
		void AddOptions(GameObject SelectedObject, ObjectEditUI ObjectEdit);
		ISettingsSaveData ConvertToSaveSettings();
	}
}
