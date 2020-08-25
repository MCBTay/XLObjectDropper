using System;
using GameManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLObjectDropper.UI;
using Object = UnityEngine.Object;

namespace XLObjectDropper.Controllers
{
	public class ObjectSelectionController : MonoBehaviour
	{
		public static ObjectSelectionUI ObjectSelection { get; set; }

		public static GameObject ListItemPrefab { get; set; }
		public event UnityAction<Spawnable> ObjectSelected = (x) => { };

		private void Awake()
		{

		}

		private void OnEnable()
		{
			ClearLists();

			// Populate List
			foreach (var item in AssetBundleHelper.LoadedSpawnables)
			{
				foreach (var spawnable in item.Value)
				{
					var listItem = Object.Instantiate(ListItemPrefab, ObjectSelection.GetListByType(item.Key).transform);
					listItem.GetComponentInChildren<TMP_Text>().SetText(spawnable.Prefab.name.Replace('_', ' '));
					listItem.GetComponent<Button>().onClick.AddListener(() => ObjectClicked(spawnable));
					listItem.GetComponent<ObjectSelectionListItem>().ListItemSelected += () => ListItemSelected(spawnable);
					listItem.SetActive(true);
				}
			}
		}

		private void ClearLists()
		{
			foreach (var item in AssetBundleHelper.LoadedSpawnables.Keys)
			{
				var listContent = ObjectSelection.GetListByType(item);

				for (var i = listContent.transform.childCount - 1; i >= 0; i--)
				{
					var objectA = listContent.transform.GetChild(i);
					objectA.transform.parent = null;
					// Optionally destroy the objectA if not longer needed
					//Destroy(objectA);
				}
			}
		}

		private static void ListItemSelected(Spawnable spawnable)
		{
			if (ObjectMovementController.PreviewObject != null && ObjectMovementController.PreviewObject.activeInHierarchy)
			{
				ObjectMovementController.PreviewObject.SetActive(false);
				Destroy(ObjectMovementController.PreviewObject);
			}

			InstantiatePreviewObject(spawnable);
		}

		private static void InstantiatePreviewObject(Spawnable spawnable)
		{
			ObjectMovementController.PreviewObject = Instantiate(spawnable.Prefab, ObjectMovementController.PinMovementController.GroundIndicator.transform);
			ObjectMovementController.PinMovementController.GroundIndicator.transform.localScale = Vector3.one;
			ObjectMovementController.PreviewObject.transform.rotation = GameStateMachine.Instance.PinObject.transform.rotation;
			ObjectMovementController.PreviewObject.transform.position = GameStateMachine.Instance.PinObject.transform.position;
			ObjectMovementController.PreviewObject.transform.ChangeLayersRecursively("Ignore Raycast");
		}

		private void ObjectClicked(Spawnable spawnable)
		{
			ObjectSelected.Invoke(spawnable);
			enabled = false;
		}
	}
}
