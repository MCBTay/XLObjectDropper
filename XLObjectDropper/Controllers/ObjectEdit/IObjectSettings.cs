using UnityEngine;
using XLObjectDropper.UI.Menus;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public interface IObjectSettings
	{
		void AddOptions(GameObject SelectedObject, ObjectEditUI ObjectEdit);
	}
}
