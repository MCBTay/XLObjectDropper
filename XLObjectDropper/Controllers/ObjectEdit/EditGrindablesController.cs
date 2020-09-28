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

			var expandable = grindableExpandable.GetComponent<GrindableSettingsExpandable>();

			if (!hasGrindables)
			{
				expandable.GrindablesToggle.Toggle.interactable = false;
			}
			else
			{
				expandable.GrindablesToggle.Toggle.interactable = true;
				expandable.GrindablesToggle.onValueChanged += (isOn) =>
				{
					// enable/disable gameobjects on grindable layer
				};
			}

			if (!hasCoping)
			{
				expandable.CopingToggle.Toggle.interactable = false;
			}
			else
			{
				expandable.CopingToggle.Toggle.interactable = true;
				expandable.CopingToggle.onValueChanged += (isOn) =>
				{
					// enable/disable gameobjects on coping layer
				};
			}
		}
	}
}
