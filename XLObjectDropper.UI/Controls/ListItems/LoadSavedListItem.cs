using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls.ListItems
{
	public class LoadSavedListItem : MonoBehaviour
	{
		public GameObject ListItem;

		public TMP_Text Name;
		public TMP_Text LastSaved;
		public TMP_Text NumObjects;

		[HideInInspector] public UnityEvent onSelect;

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
