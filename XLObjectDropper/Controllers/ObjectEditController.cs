using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLObjectDropper.Controllers.ObjectEdit;
using XLObjectDropper.UI.Controls.Expandables;
using XLObjectDropper.UI.Menus;

namespace XLObjectDropper.Controllers
{
	public class ObjectEditController : MonoBehaviour
	{
		public GameObject SelectedObject;
		public static ObjectEditUI ObjectEdit { get; set; }

		private void OnEnable()
		{
			ObjectEdit.ClearList();
		}

		private void OnDisable()
		{
			ObjectEdit.ClearList();
		}

		private void Start()
		{
			EditGeneralController.Instance.AddOptions(SelectedObject, ObjectEdit);
			EditStyleController.Instance.AddOptions(SelectedObject, ObjectEdit);
			EditLightController.Instance.AddOptions(SelectedObject, ObjectEdit);
			EditGrindablesController.Instance.AddOptions(SelectedObject, ObjectEdit);

			if (ObjectEdit.ListContent.transform.childCount > 0)
			{
				var firstChild = ObjectEdit.ListContent.transform.GetChild(0).gameObject;

				var expandable = firstChild?.GetComponent<Expandable>();

				if (expandable != null)
				{
					//TODO: It feels like there's a better way to do this.
					var general = firstChild.GetComponent<GeneralSettingsExpandable>() == null ? null : firstChild.GetComponent<GeneralSettingsExpandable>().HideInReplays.gameObject;
					var styles = firstChild.GetComponent<StyleSettingsExpandable>() == null ? null : expandable.PropertiesListContent.transform.GetChild(0).gameObject;
					var lights = firstChild.GetComponent<LightSettingsExpandable>() == null ? null : firstChild.GetComponent<LightSettingsExpandable>().EnabledToggle.gameObject;
					var grindables = firstChild.GetComponent<GrindableSettingsExpandable>() == null ? null : firstChild.GetComponent<GrindableSettingsExpandable>().GrindablesToggle.gameObject;

					var selectedObj = general == null 
						? (styles == null 
							? (lights == null 
								? grindables 
								: lights) 
							: styles)
						: general;

					if (selectedObj == null) return;

					var selectable = selectedObj.GetComponentInChildren<Selectable>(true);

					EventSystem.current.SetSelectedGameObject(selectable.gameObject);
					selectable.OnSelect(null);
				}
			}
		}

		private void Update()
		{
			if (SelectedObject == null) return;
		}
	}
}
