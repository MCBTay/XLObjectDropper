using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rewired;
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

		//private IEnumerator GetPreviewImage(Spawnable spawnable, RawImage image)
		//{
		//	var filePath = Path.Combine(Main.ModPath, spawnable.Prefab.name + ".png");

		//	Texture2D texture;

		//	if (File.Exists(filePath))
		//	{
		//		var fileData = File.ReadAllBytes(filePath);
		//		yield return fileData;

		//		texture = new Texture2D(2, 2);
		//		texture.LoadImage(fileData);
		//		yield return texture;

		//		image.texture = texture;

		//		yield break;
		//	}

		//	texture = RuntimePreviewGenerator.GenerateModelPreview(spawnable.Prefab.transform, 128, 128);
		//	yield return texture;

		//	image.texture = texture;

		//	File.WriteAllBytes(Path.Combine(Main.ModPath, spawnable.Prefab.name + ".png"), texture.EncodeToPNG());

		//	DestroyImmediate(texture);
		//}

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
			//PreviewObject = Instantiate(spawnable.Prefab);

			//PreviewObject.transform.ChangeLayersRecursively("Ignore Raycast");

			//PreviewObject.transform.position = transform.position;
			//PreviewObject.transform.rotation = spawnable.Prefab.transform.rotation;

			if (enabled)
			{
				if (ObjectMovementController.Instance.PreviewObject != null && ObjectMovementController.Instance.PreviewObject.activeInHierarchy)
				{
					ObjectMovementController.Instance.PreviewObject.SetActive(false);
					Destroy(ObjectMovementController.Instance.PreviewObject);
				}

				ObjectMovementController.Instance?.InstantiatePreviewObject(spawnable);
			}
		}

		private void ObjectClicked(Spawnable spawnable)
		{
			ObjectClickedEvent.Invoke(spawnable);
		}
	}
}
