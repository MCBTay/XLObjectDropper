using System.Collections.Generic;
using UnityEngine;
using XLObjectDropper.UI.Utilities;
using XLObjectDropper.UserInterface;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Controllers
{
	public class ObjectDropperController : MonoBehaviour
	{
		public static ObjectDropperController Instance;

		public static List<Spawnable> SpawnedObjects;

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

		private void Awake()
		{
			Instance = this;

			SpawnedObjects = new List<Spawnable>();
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
			DestroyRotationAndScaleUI();
		}

		private void Update()
		{
			var player = PlayerController.Instance.inputController.player;

			Time.timeScale = OptionsMenuOpen || ObjectSelectionOpen || QuickMenuOpen ? 0.0f : 1.0f;



			if (player.GetButtonUp("LB"))
			{
				if (RotationAndScaleOpen)
				{
					DestroyRotationAndScaleUI();
				}
			}

			if (player.GetButton("LB"))
			{
				if (player.GetButtonDown("Start"))
				{
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

				if (!QuickMenuOpen)
				{
					if (!RotationAndScaleOpen)
					{
						CreateRotationAndScaleUI();
					}
				}
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

			ObjectMovementController.Instance.CleanUp();

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

		public static void DeleteSpawnedObjects()
		{
			foreach (var spawnedObject in SpawnedObjects)
			{
				DestroyImmediate(spawnedObject.Prefab);
				DestroyImmediate(spawnedObject.SpawnedInstance);
				DestroyImmediate(spawnedObject.PreviewTexture);
			}

			SpawnedObjects.Clear();
		}
	}
}
