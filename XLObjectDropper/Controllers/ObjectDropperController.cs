using UnityEngine;
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

			DestroyObjectSelection();
			DestroyOptionsMenu();
			DestroyObjectMovement();
			DestroyQuickMenu();
			DestroyObjectEdit();
			DestroyRotationAndScaleUI();
			DestroySnappingModeUI();
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
						DestroyOptionsMenu();
						ObjectMovementController.enabled = true;
					}
					else
					{
						ObjectMovementController.enabled = false;
						CreateOptionsMenu();
					}
				}

				if (player.GetButtonDown("X") && (ObjectMovementController.SelectedObject != null || ObjectMovementController.HighlightedObject != null))
				{
					if (ObjectEditOpen)
					{
						DestroyObjectEdit();
						ObjectMovementController.enabled = true;
					}
					else
					{
						ObjectMovementController.enabled = false;
						CreateObjectEdit(ObjectMovementController.HighlightedObject ?? ObjectMovementController.SelectedObject);
					}
				}

				if (player.GetButtonTimedPressUp("Start", 0.0f, 0.7f)) // tap
				{
					UISounds.Instance.PlayOneShotSelectMajor();

					if (QuickMenuOpen)
					{
						DestroyQuickMenu();
						ObjectMovementController.enabled = true;
					}
					else
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
				}
				else if (player.GetButtonTimedPressDown("Start", 0.7f) && !ObjectSelectionOpen) //press
				{
					UISounds.Instance.PlayOneShotSelectMajor();

					if (QuickMenuOpen)
					{
						DestroyQuickMenu();
						ObjectMovementController.enabled = true;
					}
					else
					{
						ObjectMovementController.enabled = false;
						CreateQuickMenu();
					}
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

				if (QuickMenuOpen)
				{
					DestroyQuickMenu();
					ObjectMovementController.enabled = true;
				}

				if (ObjectEditOpen)
				{
					DestroyObjectEdit();
					ObjectMovementController.enabled = true;
				}
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

		#region Quick Menu
		private void CreateQuickMenu()
		{
			QuickMenuGameObject = new GameObject();
			QuickMenuController = QuickMenuGameObject.AddComponent<QuickMenuController>();
			QuickMenuController.ObjectClickedEvent += QuickMenuControllerOnObjectClickedEvent;

			QuickMenuGameObject.SetActive(true);
		}

		private void DestroyQuickMenu()
		{
			if (QuickMenuGameObject == null || QuickMenuController == null) return;

			QuickMenuGameObject.SetActive(false);
			QuickMenuController.ObjectClickedEvent -= QuickMenuControllerOnObjectClickedEvent;

			DestroyImmediate(QuickMenuController);
			DestroyImmediate(QuickMenuGameObject);
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
			ObjectMovementController.enabled = true;
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
			ObjectEditGameObject = new GameObject();
			ObjectEditController = ObjectEditGameObject.AddComponent<ObjectEditController>();
			ObjectEditController.SelectedObject = objectToEdit;

			ObjectEditGameObject.SetActive(true);
		}

		private void DestroyObjectEdit()
		{
			if (ObjectEditGameObject == null || ObjectEditController == null) return;

			ObjectEditGameObject.SetActive(false);

			DestroyImmediate(ObjectEditController);
			DestroyImmediate(ObjectEditGameObject);
		}
		#endregion
	}
}
