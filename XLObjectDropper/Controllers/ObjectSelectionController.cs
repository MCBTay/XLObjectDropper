using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using XLObjectDropper.UI;
using XLObjectDropper.Utilities;
using Object = UnityEngine.Object;

namespace XLObjectDropper.Controllers
{
	public class ObjectSelectionController : MonoBehaviour
	{
		public static ObjectSelectionUI ObjectSelection { get; set; }

		public static GameObject ListItemPrefab { get; set; }
		public event UnityAction<Spawnable> ObjectClickedEvent = (x) => { };

		private List<SpawnableType> Categories;

		public GameObject PreviewObject { get; set; }

		private void Awake()
		{
			Categories = new List<SpawnableType>();
			Categories.Add(SpawnableType.Rails);
			Categories.Add(SpawnableType.Ramps);
			Categories.Add(SpawnableType.Splines);
			Categories.Add(SpawnableType.Props);
			Categories.Add(SpawnableType.Park);
			Categories.Add(SpawnableType.Packs);
		}

		private void OnEnable()
		{
			ClearList();

			
			//RuntimePreviewGenerator.PreviewRenderCamera.enabled = true;
			//RuntimePreviewGenerator.MarkTextureNonReadable = false;
			//RuntimePreviewGenerator.OrthographicMode = false;

			// Populate List
			StartCoroutine(PopulateList());
		}

		private int CurrentCategoryIndex;

		private void Update()
		{
			var player = PlayerController.Instance.inputController.player;

			if (player.GetButtonDown("RB"))
			{
				SetActiveCategory(true);
			}

			if (player.GetButtonDown("LB"))
			{
				SetActiveCategory(false);
			}

			if (PreviewObject != null)
			{
				//RuntimePreviewGenerator.PreviewRenderCamera = AssetBundleHelper.SelfieCamera;
				//ObjectSelection.ObjectPreviewRenderTexture.GetComponent<RawImage>().texture = RuntimePreviewGenerator.GenerateModelPreview(PreviewObject.transform, 1000, 1000);

				//Vector2 rightSTick = player.GetAxis2D("RightStickX", "RightStickY");
				//PreviewObject?.transform.Rotate(0, rightSTick.x * 10, 0);
				//DestroyImmediate(RuntimePreviewGenerator.PreviewRenderCamera);
			}
		}

		private void SetActiveCategory(bool increment)
		{
			ClearList();

			if (increment) CurrentCategoryIndex++;
			else CurrentCategoryIndex--;

			if (CurrentCategoryIndex > Categories.Count - 1)
			{
				CurrentCategoryIndex = 0;
			}

			if (CurrentCategoryIndex < 0)
			{
				CurrentCategoryIndex = Categories.Count - 1;
			}

			if (AssetBundleHelper.LoadedSpawnables.Any(x => x.Type == (SpawnableType)CurrentCategoryIndex))
			{
				StartCoroutine(PopulateList());
			}
		}

		private IEnumerator PopulateList()
		{
			ClearList();

			foreach (var spawnable in AssetBundleHelper.LoadedSpawnables.Where(x => x.Type == (SpawnableType)CurrentCategoryIndex))
			{
				var listItem = Object.Instantiate(ListItemPrefab, ObjectSelection.ListContent.transform);
				listItem.GetComponentInChildren<TMP_Text>().SetText(spawnable.Prefab.name.Replace('_', ' '));
				listItem.GetComponent<Button>().onClick.AddListener(() => ObjectClicked(spawnable));
				listItem.GetComponent<ObjectSelectionListItem>().ListItemSelected += () => ListItemSelected(spawnable);

				var image = listItem.GetComponentInChildren<RawImage>();
				if (image != null)
				{
					image.texture = spawnable.PreviewTexture;
				}
				
				listItem.SetActive(true);
			}

			yield break;
		}

		private void OnDisable()
		{
			ClearList();
		}

		private void ClearList()
		{
			var listContent = ObjectSelection.ListContent;

			for (var i = listContent.transform.childCount - 1; i >= 0; i--)
			{
				var objectA = listContent.transform.GetChild(i);
				objectA.transform.parent = null;
				// Optionally destroy the objectA if not longer needed
			}
		}

		private void ListItemSelected(Spawnable spawnable)
		{
			PreviewObject = spawnable.Prefab;
		}


		private void ObjectClicked(Spawnable spawnable)
		{
			ObjectClickedEvent.Invoke(spawnable);
		}
	}
}
