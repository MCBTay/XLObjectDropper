using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.UI.Utilities;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Controllers
{
	public class ObjectSelectionController : MonoBehaviour
	{
		public static ObjectSelectionUI ObjectSelection { get; set; }

		public event UnityAction<Spawnable> ObjectClickedEvent = (x) => { };

		public GameObject PreviewObject { get; set; }

		private void OnEnable()
		{
			ObjectSelection.CategoryChanged += CategoryChanged;

			ObjectSelection.ClearList();
			PopulateList();
		}

		private void Update()
		{
			var player = PlayerController.Instance.inputController.player;

			if (PreviewObject != null)
			{
				//RuntimePreviewGenerator.PreviewRenderCamera = AssetBundleHelper.SelfieCamera;
				//ObjectSelection.ObjectPreviewRenderTexture.GetComponent<RawImage>().texture = RuntimePreviewGenerator.GenerateModelPreview(PreviewObject.transform, 1000, 1000);

				//Vector2 rightSTick = player.GetAxis2D("RightStickX", "RightStickY");
				//PreviewObject?.transform.Rotate(0, rightSTick.x * 10, 0);
				//DestroyImmediate(RuntimePreviewGenerator.PreviewRenderCamera);
			}
		}

		private void PopulateList()
		{
			ObjectSelection.ClearList();

			var spawnablesByType = SpawnableManager.Prefabs.Where(x => x.Type == (SpawnableType)ObjectSelection.CurrentCategoryIndex).ToList();
			if (spawnablesByType.Any())
			{
				foreach (var spawnable in spawnablesByType)
				{
					ObjectSelection.AddToList(spawnable.Prefab.name, spawnable.PreviewTexture, () => ObjectClicked(spawnable), () => ListItemSelected(spawnable));
				}

				if (ObjectSelection.ListContent.transform.childCount > 0)
					EventSystem.current.SetSelectedGameObject(ObjectSelection.ListContent.transform.GetChild(0).gameObject);
			}
		}

		private void OnDisable()
		{
			ObjectSelection.ClearList();

			ObjectSelection.CategoryChanged -= CategoryChanged;
		}

		private void CategoryChanged()
		{
			PopulateList();
		}

		private void ListItemSelected(Spawnable spawnable)
		{
			UISounds.Instance?.PlayOneShotSelectionChange();
			PreviewObject = spawnable.Prefab;
		}

		private void ObjectClicked(Spawnable spawnable)
		{
			UISounds.Instance?.PlayOneShotSelectMajor();
			ObjectClickedEvent.Invoke(spawnable);
		}
	}
}
