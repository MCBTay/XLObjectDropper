using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
	}
}
