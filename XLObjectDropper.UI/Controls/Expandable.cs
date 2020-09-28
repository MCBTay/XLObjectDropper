using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls
{
	public class Expandable : MonoBehaviour//, ISelectHandler, IDeselectHandler
	{
		public GameObject ExpandableItem;
		public GameObject Properties;
		public GameObject PropertiesListContent;
		public Animator Animator;
		public Button Button;
		[HideInInspector] public bool Expanded;
		[HideInInspector] public UnityEvent onSelect;

		private void OnEnable()
		{

		}

		private void OnDisable()
		{

		}

		public void Update()
		{
			bool expandableSelected = ExpandableItem == EventSystem.current.currentSelectedGameObject;
			bool childSelected = false;
			foreach (Transform child in PropertiesListContent.transform)
			{
				if (child.gameObject == EventSystem.current.currentSelectedGameObject)
				{
					childSelected = true;
				}
			}

			GetComponent<Outline>().enabled = expandableSelected || childSelected;
		}

		//public void OnSelect(BaseEventData eventData)
		//{
		//	Animator.Play("Expand");

		//	Expanded = true;
		//	Properties.SetActive(true);

		//	if (Properties.transform.childCount > 0)
		//		EventSystem.current.SetSelectedGameObject(Properties.transform.GetChild(0).gameObject);
		//}

		public void OnDeselect(BaseEventData eventData)
		{
			//Animator.Play("Collapse");

			//Expanded = false;
			//Properties.SetActive(false);
		}
	}
}
