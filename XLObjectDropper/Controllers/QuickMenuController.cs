using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.UI.Utilities;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Controllers
{
	public class QuickMenuController : MonoBehaviour
	{
		public static QuickMenuUI QuickMenu { get; set; }
		public event UnityAction<Spawnable> ObjectClickedEvent = (x) => { };

		private void OnEnable()
		{
			QuickMenu.CategoryChanged += CategoryChanged;

			QuickMenu.ClearList();
			PopulateList();
		}

		private void PopulateList()
		{
			QuickMenu.ClearList();

			List<Spawnable> objectList = new List<Spawnable>();

			switch (QuickMenu.CurrentCategoryIndex)
			{
				case (int)QuickMenuType.Recent:
					objectList = ObjectMovementController.Instance.SpawnedObjects.GroupBy(x => x.Prefab).Select(x => x.First()).ToList();
					break;
				case (int)QuickMenuType.Placed:
				default:
					objectList = ObjectMovementController.Instance.SpawnedObjects;
					break;
			}

			if (objectList != null && objectList.Any())
			{
				// oldest first
				//foreach (var spawnedObject in ObjectMovementController.Instance.SpawnedObjects)
				//{
				//QuickMenu.AddToList(spawnedObject.Prefab.name, spawnedObject.PreviewTexture, () => ObjectClicked(spawnedObject));
				//}

				// newest first
				for (int i = objectList.Count - 1; i >= 0; i--)
				{
					var spawnable = objectList[i];
					QuickMenu.AddToList(spawnable.Prefab.name, spawnable.PreviewTexture, () => ObjectClicked(spawnable), () => ObjectSelected(spawnable));
				}
			}
		}

		private void OnDisable()
		{
			QuickMenu.ClearList();

			QuickMenu.CategoryChanged -= CategoryChanged;
		}

		private void CategoryChanged()
		{
			PopulateList();
		}

		private void ObjectSelected(Spawnable spawnable)
		{
			UISounds.Instance?.PlayOneShotSelectionChange();
		}

		private void ObjectClicked(Spawnable spawnable)
		{
			UISounds.Instance?.PlayOneShotSelectMajor();
			ObjectClickedEvent.Invoke(spawnable);
		}
	}
}
