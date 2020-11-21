using System.Collections.Generic;
using UnityEngine;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities.Save.Settings;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public interface IObjectSettings
	{
		void AddOptions(GameObject SelectedObject, ObjectEditUI objectEdit);
		ISettingsSaveData ConvertToSaveSettings();
		void ApplySaveSettings(GameObject selectedObject, List<ISettingsSaveData> settings);
	}
}
