using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLObjectDropper.Controllers.ObjectEdit;
using XLObjectDropper.UI.Controls;
using XLObjectDropper.UI.Controls.Buttons;
using XLObjectDropper.UI.Controls.Expandables;
using XLObjectDropper.UI.Menus;

namespace XLObjectDropper.Controllers
{
	public class ObjectEditController : MonoBehaviour
	{
		public GameObject SelectedObject;

		public static ObjectEditUI ObjectEdit { get; set; }

		

		//public event UnityAction<Spawnable> ObjectClickedEvent = (x) => { };

		private void OnEnable()
		{
			//ObjectEdit.ClearList();
		}

		private void OnDisable()
		{
			ObjectEdit.ClearList();
		}

		private void Start()
		{
			EditStyleController.AddStyleOptions(SelectedObject, ObjectEdit);
			EditLightController.AddLightOptions(SelectedObject, ObjectEdit);
			EditGrindablesController.AddGrindablesOptions(SelectedObject, ObjectEdit);

			if (ObjectEdit.ListContent.transform.childCount > 0)
			{
				var firstChild = ObjectEdit.ListContent.transform.GetChild(0).gameObject;

					var animator = firstChild?.GetComponent<Animator>();

					if (animator != null)
					{
						animator.Play("Expand");
					}

					var expandable = firstChild?.GetComponent<Expandable>();

					if (expandable != null)
					{
						expandable.Properties.SetActive(true);
						expandable.Expanded = true;

						var styles = firstChild.GetComponent<StyleSettingsExpandable>();
						var lights = firstChild.GetComponent<LightSettingsExpandable>();
						var grindables = firstChild.GetComponent<GrindableSettingsExpandable>();

						var selectedObj =
							styles != null ?
								expandable.PropertiesListContent.transform.GetChild(0).gameObject :
								(lights != null ? lights.EnabledToggle.gameObject :
									(grindables != null ? grindables.GrindablesToggle.gameObject : null));

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
