using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLObjectDropper.UI.Controls;
using Color = XLObjectDropper.UI.Utilities.Color;

namespace XLObjectDropper.UI.Menus
{
	public class LoadSavedUI : MonoBehaviour
	{
		public GameObject ListContent;
		public GameObject SaveItemPrefab;
		public GameObject UnsavedChangesDialog;
		public Animator Animator;

		private void OnEnable()
		{
			UnsavedChangesDialog.SetActive(false);

			Animator.Play("SlideIn");
		}

		public virtual void ClearList()
		{
			for (var i = ListContent.transform.childCount - 1; i >= 0; i--)
			{
				var objectA = ListContent.transform.GetChild(i);
				objectA.transform.parent = null;

				Destroy(objectA.gameObject);
			}

			EventSystem.current.SetSelectedGameObject(null);
		}

		public GameObject AddToList(string name, DateTime date, int numObjects, UnityAction objectClicked = null, UnityAction objectSelected = null)
		{
			var listItem = Instantiate(SaveItemPrefab, ListContent.transform);

			var saveListItem = listItem.GetComponent<LoadSavedListItem>();
			saveListItem.Name.SetText(name);
			saveListItem.LastSaved.SetText($"Last Saved:{Environment.NewLine}{Color.ColorTag}{date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern)} {date.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern)}");
			saveListItem.NumObjects.SetText($"{Color.ColorTag}{numObjects} <color=#FFFFFFFF>Objects");

			if (objectClicked != null)
			{
				listItem.GetComponent<Button>().onClick.AddListener(objectClicked);
			}

			if (objectSelected != null)
			{
				listItem.GetComponent<LoadSavedListItem>().onSelect.AddListener(objectSelected);
			}

			listItem.SetActive(true);

			return listItem;
		}

		public UnsavedChangesDialog CreateUnsavedChangesDialog()
		{
			UnsavedChangesDialog.SetActive(true);

			var unsavedChanges = UnsavedChangesDialog.GetComponent<UnsavedChangesDialog>();

			EventSystem.current.SetSelectedGameObject(unsavedChanges.YesButton);

			return unsavedChanges;
		}

		public void DestroyUnsavedChangesDialog()
		{
			UnsavedChangesDialog.SetActive(false);
		}
	}
}
