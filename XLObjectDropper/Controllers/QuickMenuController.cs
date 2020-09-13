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

			switch (QuickMenu.CurrentCategoryIndex)
			{
				case (int)QuickMenuType.Recent:
					break;
				case (int)QuickMenuType.Placed:
				default:
					if (ObjectMovementController.Instance.SpawnedObjects.Any())
					{
						foreach (var spawnedObject in ObjectMovementController.Instance.SpawnedObjects)
						{
							QuickMenu.AddToList(spawnedObject.Prefab.name, spawnedObject.PreviewTexture, () => ObjectClicked(spawnedObject));
						}
					}
					break;
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

		private void ObjectClicked(Spawnable spawnable)
		{
			ObjectClickedEvent.Invoke(spawnable);
		}
	}
}
