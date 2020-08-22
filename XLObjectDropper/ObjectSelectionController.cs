using System.Collections.Generic;
using System.Linq;
using GameManagement;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityModManagerNet;
using XLObjectDropper.UI;

namespace XLObjectDropper
{
	public class ObjectSelectionController : MonoBehaviour, ISelectHandler
	{
		public static ObjectSelectionUI ObjectSelection { get; set; }

		public static GameObject ListItemPrefab { get; set; }

		private void Awake()
		{

		}

		private void OnEnable()
		{
			// Clear list
			for (var i = ObjectSelection.ListContent.transform.childCount - 1; i >= 0; i--)
			{
				// objectA is not the attached GameObject, so you can do all your checks with it.
				var objectA = ObjectSelection.ListContent.transform.GetChild(i);
				objectA.transform.parent = null;
				// Optionally destroy the objectA if not longer needed
			}

			if (ListItemPrefab != null)
			{
				// Populate List
				foreach (var item in AssetBundleHelper.LoadedAssets)
				{
					var listItem = Object.Instantiate(ListItemPrefab, ObjectSelection.ListContent.transform);
					listItem.GetComponentInChildren<TMP_Text>().SetText(item.name);
					listItem.GetComponent<Button>().onClick.AddListener(() => ObjectSelected(item));

					listItem.SetActive(true);
				}
			}
		}

		private void ObjectSelected(GameObject gameObject)
		{
			UnityModManager.Logger.Log("You clicked an item: " + gameObject.name);
			// Exit object selection state, set this item as the preview object
		}

		private void OnDisable()
		{
			
		}

		public void OnSelect(BaseEventData eventData)
		{
			UnityModManager.Logger.Log(this.gameObject.name + " was selected");
		}

		private void Update()
		{
			
		}
	}

	public class ObjectInfo
	{
		public string name;
	}

	public class ObjectListItem : ListViewItem<ObjectInfo>
	{
		public TMP_Text ObjectNameText;
		private ObjectInfo @object;

		public override ObjectInfo Item
		{
			get
			{
				return @object;
			}
			set
			{
				@object = value;
				ObjectNameText.text = this.@object.name;
			}
		}
	}
}
