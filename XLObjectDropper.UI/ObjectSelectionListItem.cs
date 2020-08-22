using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XLObjectDropper.UI
{
	public class ObjectSelectionListItem : MonoBehaviour, ISelectHandler
	{
		public GameObject ListItem;

		[HideInInspector] public event UnityAction ListItemSelected = () => { };

		public void OnEnable()
		{
			ListItem.GetComponent<Button>()?.onClick.AddListener(ListItemButtonClicked);
		}

		public void ListItemButtonClicked()
		{
			UIManager.Instance.ObjectSelectionUI.SetActive(false);
			UIManager.Instance.ObjectPlacementUI.SetActive(true);
		}

		public void OnDisable()
		{
			ListItem.GetComponent<Button>()?.onClick.RemoveListener(ListItemButtonClicked);
		}

		public void OnSelect(BaseEventData eventData)
		{
			ListItemSelected.Invoke();
		}
	}
}
