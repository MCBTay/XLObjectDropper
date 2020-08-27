using GameManagement;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using XLObjectDropper.UI;
using Object = UnityEngine.Object;

namespace XLObjectDropper.Controllers
{
	public class ObjectSelectionController : MonoBehaviour
	{
		public static ObjectSelectionUI ObjectSelection { get; set; }

		public static GameObject ListItemPrefab { get; set; }
		public event UnityAction<Spawnable> ObjectClickedEvent = (x) => { };

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

		private void OnDisable()
		{
			ClearLists();
		}

		private void ClearLists()
		{
			foreach (var item in AssetBundleHelper.LoadedSpawnables)
			{
				var listContent = ObjectSelection.GetListByType(item.Key);

				for (var i = listContent.transform.childCount - 1; i >= 0; i--)
				{
					var objectA = listContent.transform.GetChild(i);
					objectA.transform.parent = null;
					// Optionally destroy the objectA if not longer needed
				}
			}
		}

		private void ListItemSelected(Spawnable spawnable)
		{
			if (enabled)
			{
				if (ObjectMovementController.PreviewObject != null && ObjectMovementController.PreviewObject.activeInHierarchy)
				{
					ObjectMovementController.PreviewObject.SetActive(false);
					Destroy(ObjectMovementController.PreviewObject);
				}

				InstantiatePreviewObject(spawnable);
			}
		}

		private static void InstantiatePreviewObject(Spawnable spawnable)
		{
			ObjectMovementController.PreviewObject = Instantiate(spawnable.Prefab, ObjectMovementController.PinMovementController.GroundIndicator.transform);
			
			ObjectMovementController.PinMovementController.GroundIndicator.transform.localScale = Vector3.one;

			GameStateMachine.Instance.PinObject.GetComponentsInChildren<MeshRenderer>(true).FirstOrDefault(x => x.name == "GroundLocationIndicator").enabled = false;
			
			ObjectMovementController.PreviewObject.transform.ChangeLayersRecursively("Ignore Raycast");

			ObjectMovementController.PreviewObject.transform.position = ObjectMovementController.PinMovementController.GroundIndicator.transform.position;
			ObjectMovementController.PreviewObject.transform.rotation = spawnable.Prefab.transform.rotation;
		}

		private void ObjectClicked(Spawnable spawnable)
		{
			ObjectClickedEvent.Invoke(spawnable);
		}
	}
}
