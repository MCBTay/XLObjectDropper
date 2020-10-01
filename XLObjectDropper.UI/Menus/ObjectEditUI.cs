using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLObjectDropper.UI.Controls;

namespace XLObjectDropper.UI.Menus
{
	public class ObjectEditUI : MonoBehaviour
	{
		public GameObject ListContent;
		public GameObject ExpandablePrefab;

		public GameObject StyleSettingsPrefab;
		public GameObject LightSettingsPrefab;
		public GameObject GrindableSettingsPrefab;

		public Animator Animator;

		private void OnEnable()
		{
			Animator.Play("SlideIn");
		}

		public GameObject AddLightSettings(UnityAction objectClicked = null, UnityAction objectSelected = null)
		{
			return AddSettings(LightSettingsPrefab, objectClicked, objectSelected);
		}

		public GameObject AddGrindableSettings(UnityAction objectClicked = null, UnityAction objectSelected = null)
		{
			return AddSettings(GrindableSettingsPrefab, objectClicked, objectSelected);
		}

		public GameObject AddStyleSettings(UnityAction objectClicked = null, UnityAction objectSelected = null)
		{
			return AddSettings(StyleSettingsPrefab, objectClicked, objectSelected);
		}

		public GameObject AddSettings(GameObject prefab, UnityAction objectClicked = null, UnityAction objectSelected = null)
		{
			var listItem = Instantiate(prefab, ListContent.transform);

			if (objectClicked != null)
			{
				listItem.GetComponent<Button>().onClick.AddListener(objectClicked);
			}

			if (objectSelected != null)
			{
				listItem.GetComponent<ObjectSelectionListItem>().onSelect.AddListener(objectSelected);
			}

			listItem.SetActive(true);

			return listItem;
		}

		public virtual void ClearList()
		{
			for (var i = ListContent.transform.childCount - 1; i >= 0; i--)
			{
				var objectA = ListContent.transform.GetChild(i);
				objectA.transform.parent = null;

				//objectA.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
				//objectA.gameObject.GetComponent<ObjectSelectionListItem>().onSelect.RemoveAllListeners();

				Destroy(objectA.gameObject);
			}

			EventSystem.current.SetSelectedGameObject(null);
		}
	}
}
