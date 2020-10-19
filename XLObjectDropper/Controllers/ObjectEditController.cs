using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLObjectDropper.UI.Controls.Expandables;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities;

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
			var spawnable = SelectedObject.GetSpawnableFromSpawned() ?? SelectedObject.GetSpawnable();
			if (spawnable == null) return;

			foreach (var settings in spawnable.Settings)
			{
				settings.AddOptions(SelectedObject, ObjectEdit);
			}

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
	}
}
