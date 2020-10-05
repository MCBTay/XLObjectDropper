using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls
{
	public class ObjectSelectionListItem : MonoBehaviour, ISelectHandler
	{
		public GameObject ListItem;
		[HideInInspector] public UnityEvent onSelect;

		public void OnSelect(BaseEventData eventData)
		{
			onSelect.Invoke();
		}

		public void Update()
		{
			GetComponent<Outline>().enabled = false;

			if (ListItem == EventSystem.current.currentSelectedGameObject)
			{
				GetComponent<Outline>().enabled = true;
			}
		}
	}
}
