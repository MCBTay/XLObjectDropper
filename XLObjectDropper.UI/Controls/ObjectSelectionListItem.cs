using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls
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
			if (UIManager.Instance.ObjectSelectionUI != null && UIManager.Instance.ObjectSelectionUI.activeInHierarchy)
			{
				UIManager.Instance.ObjectSelectionUI.SetActive(false);
			}
			else if (UIManager.Instance.QuickMenuUI != null && UIManager.Instance.QuickMenuUI.activeInHierarchy)
			{
				UIManager.Instance.QuickMenuUI.SetActive(false);
			}

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
