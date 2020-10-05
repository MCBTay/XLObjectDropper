using UnityEngine;
using XLObjectDropper.UI;
using XLObjectDropper.UI.Utilities;
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

		private GameObject QuickMenuGameObject;
		private QuickMenuController QuickMenuController;
		private bool QuickMenuOpen => QuickMenuGameObject != null && QuickMenuGameObject.activeInHierarchy;

		private GameObject RotationAndScaleGameObject;
		private RotationAndScaleController RotationAndScaleController;
		private bool RotationAndScaleOpen => RotationAndScaleGameObject != null && RotationAndScaleGameObject.activeInHierarchy;

		private GameObject SnappingModeGameObject;
		private SnappingModeController SnappingModeController;
		private bool SnappingModeOpen => SnappingModeGameObject != null && SnappingModeGameObject.activeInHierarchy;

		private GameObject ObjectEditGameObject;
		private ObjectEditController ObjectEditController;
		private bool ObjectEditOpen => ObjectEditGameObject != null && ObjectEditGameObject.activeInHierarchy;

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

			DestroyObjectMovement();
			DestroyRotationAndScaleUI();
			DestroySnappingModeUI();
			DestroyObjectSelection(false);
			DestroyOptionsMenu(false);
			DestroyQuickMenu(false);
			DestroyObjectEdit(false);
		}

		private void Update()
		{
			var player = PlayerController.Instance.inputController.player;

			Time.timeScale = OptionsMenuOpen || ObjectSelectionOpen || QuickMenuOpen ? 0.0f : 1.0f;

			if (player.GetButtonUp("LB") && RotationAndScaleOpen) DestroyRotationAndScaleUI();
			if (player.GetButtonUp("RB") && SnappingModeOpen) DestroySnappingModeUI();

			if (player.GetButton("LB") && !RotationAndScaleOpen && ObjectMovementController.SelectedObjectActive)
			{
				CreateRotationAndScaleUI();
			}
			else if (player.GetButton("RB") && !SnappingModeOpen && ObjectMovementController.SelectedObjectActive)
			{
				CreateSnappingModeUI();
			}
			else
			{
				if (player.GetButtonDown("Select"))
				{
					if (OptionsMenuOpen)
					{
						if (OptionsMenuController.LoadSavedOpen) 
							return;

						DestroyOptionsMenu();
					}
					else
					{
						CreateOptionsMenu();
					}
				}

				if (player.GetButtonDown("X") && (ObjectMovementController.SelectedObject != null || ObjectMovementController.HighlightedObject != null))
				{
					if (ObjectEditOpen) DestroyObjectEdit();
					else CreateObjectEdit(ObjectMovementController.HighlightedObject ?? ObjectMovementController.SelectedObject);
				}

				if (player.GetButtonTimedPressUp("Start", 0.0f, 0.7f)) // tap
				{
					if (QuickMenuOpen)
					{
						DestroyQuickMenu();
					}
					else
					{
						if (ObjectSelectionOpen) DestroyObjectSelection();
						else CreateObjectSelection();
					}
				}
				else if (player.GetButtonTimedPressDown("Start", 0.7f) && !ObjectSelectionOpen) //press
				{
					if (QuickMenuOpen) DestroyQuickMenu();
					else CreateQuickMenu();
				}
			}

			if (player.GetButtonDown("B"))
			{
				if (OptionsMenuOpen)
				{
					if (OptionsMenuController.LoadSavedOpen) 
						return;

					DestroyOptionsMenu();
				}

				if (ObjectSelectionOpen) DestroyObjectSelection();
				if (QuickMenuOpen) DestroyQuickMenu();
				if (ObjectEditOpen) DestroyObjectEdit();
			}
		}

		#region Object Movement
		private void CreateObjectMovement()
		{
			if (ObjectMovementGameObject == null)
			{
				ObjectMovementGameObject = new GameObject();
				Object.DontDestroyOnLoad(ObjectMovementGameObject);
			}

			if (ObjectMovementController == null)
			{
				ObjectMovementController = ObjectMovementGameObject.AddComponent<ObjectMovementController>();
			}
			
			ObjectMovementGameObject.SetActive(true);
		}

		private void DestroyObjectMovement()
		{
			if (ObjectMovementController == null) return;

			ObjectMovementController.enabled = false;
			ObjectMovementController.CleanUp();
		}
		#endregion

		#region Object Selection
		private void CreateObjectSelection()
		{
			ObjectMovementController.enabled = false;

			UISounds.Instance?.PlayOneShotSelectMajor();

			ObjectSelectionMenuGameObject = new GameObject();
			ObjectSelectionController = ObjectSelectionMenuGameObject.AddComponent<ObjectSelectionController>();
			ObjectSelectionController.ObjectClickedEvent += ObjectSelectionControllerOnObjectClickedEvent;

			ObjectSelectionMenuGameObject.SetActive(true);
		}

		private void DestroyObjectSelection(bool reenableObjMovement = true)
		{
			if (ObjectSelectionMenuGameObject == null || ObjectSelectionController == null) return;

			UISounds.Instance?.PlayOneShotSelectMajor();

			ObjectSelectionMenuGameObject.SetActive(false);
			ObjectSelectionController.ObjectClickedEvent -= ObjectSelectionControllerOnObjectClickedEvent;

			DestroyImmediate(ObjectSelectionController);
			DestroyImmediate(ObjectSelectionMenuGameObject);

			if (reenableObjMovement)
			{
				ObjectMovementController.enabled = true;
			}
		}

		private void ObjectSelectionControllerOnObjectClickedEvent(Spawnable spawnable)
		{
			ObjectMovementController.InstantiateSelectedObject(spawnable);
			
			DestroyObjectSelection();
		}
		#endregion

		#region Options Menu
		private void CreateOptionsMenu()
		{
			ObjectMovementController.enabled = false;

			UISounds.Instance?.PlayOneShotSelectMajor();

			OptionsMenuGameObject = new GameObject();
			OptionsMenuController = OptionsMenuGameObject.AddComponent<OptionsMenuController>();

			OptionsMenuController.SaveLoaded += () =>
			{
				StartCoroutine(UIManager.Instance.DisableOptionsMenu());
				DestroyOptionsMenu();
			};

			OptionsMenuGameObject.SetActive(true);
		}

		public void DestroyOptionsMenu(bool reenableObjMovement = true)
		{
			if (OptionsMenuGameObject == null || OptionsMenuController == null) return;
			
			OptionsMenuGameObject.SetActive(false);

			DestroyImmediate(OptionsMenuController);
			DestroyImmediate(OptionsMenuGameObject);

			if (reenableObjMovement)
			{
				ObjectMovementController.enabled = true;
			}
		}
		#endregion

		#region Quick Menu
		private void CreateQuickMenu()
		{
			ObjectMovementController.enabled = false;

			UISounds.Instance?.PlayOneShotSelectMajor();

			QuickMenuGameObject = new GameObject();
			QuickMenuController = QuickMenuGameObject.AddComponent<QuickMenuController>();
			QuickMenuController.ObjectClickedEvent += QuickMenuControllerOnObjectClickedEvent;

			QuickMenuGameObject.SetActive(true);
		}

		private void DestroyQuickMenu(bool reenableObjMovement = true)
		{
			if (QuickMenuGameObject == null || QuickMenuController == null) return;

			UISounds.Instance?.PlayOneShotSelectMajor();

			QuickMenuGameObject.SetActive(false);
			QuickMenuController.ObjectClickedEvent -= QuickMenuControllerOnObjectClickedEvent;

			DestroyImmediate(QuickMenuController);
			DestroyImmediate(QuickMenuGameObject);

			if (reenableObjMovement)
			{
				ObjectMovementController.enabled = true;
			}
		}

		private void QuickMenuControllerOnObjectClickedEvent(Spawnable spawnable)
		{
			switch (QuickMenuController.QuickMenu.CurrentCategoryIndex)
			{
				case (int)QuickMenuType.Recent:
					ObjectMovementController.InstantiateSelectedObject(spawnable);
					break;
				case (int)QuickMenuType.Placed:
				default:
					ObjectMovementController.cameraPivot.position = spawnable.SpawnedInstance.transform.position;
					ObjectMovementController.MoveCamera(true);
					break;
			}

			DestroyQuickMenu();
		}
		#endregion

		#region Rotation And Scale UI
		private void CreateRotationAndScaleUI()
		{
			RotationAndScaleGameObject = new GameObject();
			RotationAndScaleController = RotationAndScaleGameObject.AddComponent<RotationAndScaleController>();

			RotationAndScaleGameObject.SetActive(true);
		}

		private void DestroyRotationAndScaleUI()
		{
			if (RotationAndScaleGameObject == null || RotationAndScaleController == null) return;

			RotationAndScaleGameObject.SetActive(false);

			DestroyImmediate(RotationAndScaleController);
			DestroyImmediate(RotationAndScaleGameObject);
		}
		#endregion

		#region Snapping Mode UI
		private void CreateSnappingModeUI() {
			SnappingModeGameObject = new GameObject();
			SnappingModeController = SnappingModeGameObject.AddComponent<SnappingModeController>();

			SnappingModeGameObject.SetActive(true);
		}

		private void DestroySnappingModeUI()
		{
			if (SnappingModeGameObject == null || SnappingModeController == null) return;

			SnappingModeGameObject.SetActive(false);

			DestroyImmediate(SnappingModeController);
			DestroyImmediate(SnappingModeGameObject);
		}
		#endregion

		#region Object Edit
		private void CreateObjectEdit(GameObject objectToEdit)
		{
			ObjectMovementController.enabled = false;

			UISounds.Instance?.PlayOneShotSelectMajor();

			ObjectEditGameObject = new GameObject();
			ObjectEditController = ObjectEditGameObject.AddComponent<ObjectEditController>();
			ObjectEditController.SelectedObject = objectToEdit;

			ObjectEditGameObject.SetActive(true);
		}

		private void DestroyObjectEdit(bool reenableObjMovement = true)
		{
			if (ObjectEditGameObject == null || ObjectEditController == null) return;

			UISounds.Instance?.PlayOneShotSelectMajor();

			ObjectEditGameObject.SetActive(false);

			DestroyImmediate(ObjectEditController);
			DestroyImmediate(ObjectEditGameObject);

			if (reenableObjMovement)
			{
				ObjectMovementController.enabled = true;
			}
		}
		#endregion
	}
}
