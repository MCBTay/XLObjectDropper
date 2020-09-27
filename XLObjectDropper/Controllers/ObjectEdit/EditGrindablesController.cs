using System.Linq;
using UnityEngine;
using XLObjectDropper.UI.Controls;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public class EditGrindablesController
	{
		public static void AddGrindablesOptions(GameObject SelectedObject, ObjectEditUI ObjectEdit)
		{
			if (ObjectMovementController.Instance.SelectedObjectLayerInfo == null) return;

			var grindables = SelectedObject.GetPrefab().GetChildrenOnLayer("Grindable");
			bool hasGrindables = grindables != null && grindables.Any();

			var copings = SelectedObject.GetPrefab().GetChildrenOnLayer("Coping");
			bool hasCoping = copings != null && copings.Any();

			if (!hasGrindables && !hasCoping) return;

			var grindableExpandable = ObjectEdit.AddGrindableSettings();

			var expandable = grindableExpandable.GetComponent<Expandable>();

			//if (hasGrindables)
			//{
			//	expandable.AddToggle("Grindables Enabled", true, (isOn) =>
			//	{
			//		foreach (var grindable in grindables)
			//		{
			//			grindable.SetActive(isOn);
			//		}
			//	});
			//}

			//if (hasCoping)
			//{
			//	expandable.AddToggle("Coping Enabled", true, (isOn) =>
			//	{
			//		foreach (var coping in copings)
			//		{
			//			coping.SetActive(isOn);
			//		}
			//	});
			//}
		}
	}
}
