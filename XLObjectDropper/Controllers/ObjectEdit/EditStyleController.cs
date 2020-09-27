using System.Linq;
using UnityEngine;
using XLObjectDropper.SpawnableScripts;
using XLObjectDropper.UI.Controls;
using XLObjectDropper.UI.Menus;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public static class EditStyleController
	{
		public static void AddStyleOptions(GameObject SelectedObject, ObjectEditUI ObjectEdit)
		{
			var group = SelectedObject.GetComponent<StyleGroupController>();

			if (group == null || group.Objects == null || !group.Objects.Any()) return;

			var styleExpandable = ObjectEdit.AddToList();
			var expandable = styleExpandable.GetComponent<Expandable>();

			if (expandable != null)
			{
				
			}
		}
	}
}
