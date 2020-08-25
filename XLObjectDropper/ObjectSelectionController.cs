using GameManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLObjectDropper.UI;

namespace XLObjectDropper
{
	public class ObjectSelectionController : MonoBehaviour
	{
		public static ObjectSelectionUI ObjectSelection { get; set; }

		public static GameObject ListItemPrefab { get; set; }
		public event UnityAction ObjectSelected = () => { };

		private void Awake()
		{

		}

		private void OnEnable()
		{
			ClearList();

			// Populate List
			foreach (var item in AssetBundleHelper.LoadedAssets)
			{
				var listItem = Object.Instantiate(ListItemPrefab, ObjectSelection.ListContent.transform);
				listItem.GetComponentInChildren<TMP_Text>().SetText(item.name.Replace('_', ' '));
				listItem.GetComponent<Button>().onClick.AddListener(() => ObjectClicked(item));
				listItem.GetComponent<ObjectSelectionListItem>().ListItemSelected += () => ListItemSelected(item);
				listItem.SetActive(true);
			}
		}

		private void ClearList()
		{
			for (var i = ObjectSelection.ListContent.transform.childCount - 1; i >= 0; i--)
			{
				var objectA = ObjectSelection.ListContent.transform.GetChild(i);
				objectA.transform.parent = null;
				// Optionally destroy the objectA if not longer needed
				//Destroy(objectA);
			}
		}

		private static void ListItemSelected(GameObject prefab)
		{
			if (ObjectMovementController.PreviewObject != null && ObjectMovementController.PreviewObject.activeInHierarchy)
			{
				foreach (var item in AssetBundleHelper.LoadedAssets)
				{
					ObjectMovementController.PreviewObject.SetActive(false);
					Destroy(ObjectMovementController.PreviewObject);
				}
			}

			InstantiatePreviewObject(prefab);
		}

		private static void InstantiatePreviewObject(GameObject prefab)
		{
			ObjectMovementController.PreviewObject = Instantiate(prefab, ObjectMovementController.PinMovementController.GroundIndicator.transform);
			ObjectMovementController.PinMovementController.GroundIndicator.transform.localScale = Vector3.one;
			ObjectMovementController.PreviewObject.transform.rotation = GameStateMachine.Instance.PinObject.transform.rotation;
			ObjectMovementController.PreviewObject.transform.position = GameStateMachine.Instance.PinObject.transform.position;
			ObjectMovementController.PreviewObject.transform.ChangeLayersRecursively("Ignore Raycast");
		}

		private void ObjectClicked(GameObject prefab)
		{
			ObjectSelected.Invoke();
			enabled = false;
		}
	}
}
