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
			EditAvatarAnimatorController.Instance.AddOptions(SelectedObject, ObjectEdit);
			EditColorTintController.Instance.AddOptions(SelectedObject, ObjectEdit);

			if (ObjectEdit.ListContent.transform.childCount > 0)
			{
				var firstChild = ObjectEdit.ListContent.transform.GetChild(0).gameObject;

				var expandable = firstChild?.GetComponent<Expandable>();

				if (expandable != null)
				{
					var general = firstChild.GetComponent<GeneralSettingsExpandable>() == null ? null : firstChild.GetComponent<GeneralSettingsExpandable>().HideInReplays.gameObject;
					if (general == null) return;

					var selectable = general.GetComponentInChildren<Selectable>(true);

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
