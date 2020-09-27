using System.Collections.Generic;
using TMPro;
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

		public GameObject AddToList(string name, UnityAction objectClicked = null, UnityAction objectSelected = null)
		{
			var listItem = Instantiate(ExpandablePrefab, ListContent.transform);

			listItem.GetComponentInChildren<TMP_Text>().SetText(name.Replace('_', ' '));

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

				objectA.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
				//objectA.gameObject.GetComponent<ObjectSelectionListItem>().onSelect.RemoveAllListeners();

				Destroy(objectA.gameObject);
			}

			EventSystem.current.SetSelectedGameObject(null);
		}
	}
}
