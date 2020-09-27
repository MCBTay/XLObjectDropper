using UnityEngine;
using UnityEngine.EventSystems;
using XLObjectDropper.Controllers.ObjectEdit;
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

		}

		private void OnDisable()
		{
			SelectedObject = null;
		}

		private void Start()
		{
			EditStyleController.AddStyleOptions(SelectedObject, ObjectEdit);
			EditLightController.AddLightOptions(SelectedObject, ObjectEdit);
			EditGrindablesController.AddGrindablesOptions(SelectedObject, ObjectEdit);


			if (ObjectEdit.ListContent.transform.childCount > 0)
				EventSystem.current.SetSelectedGameObject(ObjectEdit.ListContent.transform.GetChild(0).gameObject);
		}

		private void Update()
		{
			if (SelectedObject == null) return;
		}
	}
}
