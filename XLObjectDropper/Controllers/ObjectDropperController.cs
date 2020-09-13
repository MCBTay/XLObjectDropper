using UnityEngine;
using XLObjectDropper.UserInterface;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Controllers
{
	public class ObjectDropperController : MonoBehaviour
	{
		public static ObjectDropperController Instance;

		private GameObject ObjectMovementGameObject;
		private ObjectMovementController ObjectMovementController;

		private GameObject OptionsMenuGameObject;
		private OptionsMenuController OptionsMenuController;
		private bool OptionsMenuOpen => OptionsMenuGameObject != null && OptionsMenuGameObject.activeInHierarchy;

		private GameObject ObjectSelectionMenuGameObject;
		private ObjectSelectionController ObjectSelectionController;
		private bool ObjectSelectionOpen => ObjectSelectionMenuGameObject != null && ObjectSelectionMenuGameObject.activeInHierarchy;

		private void Awake()
		{
			Instance = this;
		}

		private void OnEnable()
		{
			UserInterfaceHelper.UserInterface?.SetActive(true);

			CreateObjectMovement();
		}

		private void OnDisable()
		{
			UserInterfaceHelper.UserInterface?.SetActive(false);

			DestroyObjectSelection();
			DestroyOptionsMenu();
			DestroyObjectMovement();
		}

		private void Update()
		{
			var player = PlayerController.Instance.inputController.player;

			Time.timeScale = OptionsMenuOpen || ObjectSelectionOpen ? 0.0f : 1.0f;

			if (player.GetButtonDown("Select"))
			{
				if (OptionsMenuOpen)
				{
					DestroyOptionsMenu();
					ObjectMovementController.enabled = true;
				}
				else
				{
					ObjectMovementController.enabled = false;
					CreateOptionsMenu();
				}
			}

			if (player.GetButtonDown("Start"))
			{
				if (ObjectSelectionOpen)
				{
					DestroyObjectSelection();
					ObjectMovementController.enabled = true;
				}
				else
				{
					ObjectMovementController.enabled = false;
					CreateObjectSelection();
				}
			}

			if (player.GetButtonDown("B"))
			{
				if (OptionsMenuOpen)
				{
					DestroyOptionsMenu();
					ObjectMovementController.enabled = true;
				}

				if (ObjectSelectionOpen)
				{
					DestroyObjectSelection();
					ObjectMovementController.enabled = true;
				}
			}
		}

		#region Object Movement
		private void CreateObjectMovement()
		{
			ObjectMovementGameObject = new GameObject();
			ObjectMovementController = ObjectMovementGameObject.AddComponent<ObjectMovementController>();

			ObjectMovementGameObject.SetActive(true);
		}

		private void DestroyObjectMovement()
		{
			if (ObjectMovementGameObject == null || ObjectMovementController == null) return;

			ObjectMovementGameObject.SetActive(false);

			DestroyImmediate(ObjectMovementController);
			DestroyImmediate(ObjectMovementGameObject);
		}
		#endregion

		#region Object Selection
		private void CreateObjectSelection()
		{
			ObjectSelectionMenuGameObject = new GameObject();
			ObjectSelectionController = ObjectSelectionMenuGameObject.AddComponent<ObjectSelectionController>();
			ObjectSelectionController.ObjectClickedEvent += ObjectSelectionControllerOnObjectClickedEvent;

			ObjectSelectionMenuGameObject.SetActive(true);
		}

		private void DestroyObjectSelection()
		{
			if (ObjectSelectionMenuGameObject == null || ObjectSelectionController == null) return;

			ObjectSelectionMenuGameObject.SetActive(false);
			ObjectSelectionController.ObjectClickedEvent -= ObjectSelectionControllerOnObjectClickedEvent;

			DestroyImmediate(ObjectSelectionController);
			DestroyImmediate(ObjectSelectionMenuGameObject);
		}

		private void ObjectSelectionControllerOnObjectClickedEvent(Spawnable spawnable)
		{
			ObjectMovementController.InstantiateSelectedObject(spawnable);
			
			DestroyObjectSelection();
			ObjectMovementController.enabled = true;
		}
		#endregion

		#region Options Menu
		private void CreateOptionsMenu()
		{
			OptionsMenuGameObject = new GameObject();
			OptionsMenuController = OptionsMenuGameObject.AddComponent<OptionsMenuController>();

			OptionsMenuGameObject.SetActive(true);
		}

		private void DestroyOptionsMenu()
		{
			if (OptionsMenuGameObject == null || OptionsMenuController == null) return;

			OptionsMenuGameObject.SetActive(false);

			DestroyImmediate(OptionsMenuController);
			DestroyImmediate(OptionsMenuGameObject);
		}
		#endregion
	}
}
