using GameManagement;
using Rewired;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using XLObjectDropper.GameManagement;
using XLObjectDropper.UserInterface;

namespace XLObjectDropper
{
	public class ObjectMovementController : MonoBehaviour
	{
		public static GameObject PreviewObject { get; set; }
        public List<GameObject> SpawnedObjects { get; set; }
        
		private TMP_Text ZoomInOutText { get; set; }
		public static PinMovementController PinMovementController { get; set; }
		private static GameObject OriginalPinObject { get; set; }

		private static bool OptionsMenuShown { get; set; }
		private static GameObject OptionsMenuGameObject;
		private static OptionsMenuController OptionsMenuController { get; set; }

		private static bool ObjectSelectionShown { get; set; }
		private static bool ObjectSelected { get; set; }
		private static GameObject ObjectSelectionGameObject;
		private static ObjectSelectionController ObjectSelectionController { get; set; }

		public static ObjectMovementController Instance { get; set; }

		private int counter = 0;

		private void Awake()
		{
			Instance = this;

	        SpawnedObjects = new List<GameObject>();

	        PinMovementController = GameStateMachine.Instance.PinObject.GetComponent<PinMovementController>();

	        UserInterfaceHelper.LoadUserInterface();

			OptionsMenuGameObject = new GameObject();
			OptionsMenuController = OptionsMenuGameObject.AddComponent<OptionsMenuController>();

			ObjectSelectionGameObject = new GameObject();
			ObjectSelectionController = OptionsMenuGameObject.AddComponent<ObjectSelectionController>();
			ObjectSelectionController.ObjectSelected += ObjectSelectionControllerOnObjectSelected;

			//InstantiatePreviewObject();

			//PreviewObject.SetActive(false);
			
	        //DontDestroyOnLoad(PreviewObject);

	        
	        OriginalPinObject = GameStateMachine.Instance.PinObject;

	        if (!(GameStateMachine.Instance.CurrentState.GetType() != typeof(ObjectMovementState)))
				return;

			enabled = false;
        }

		private void ObjectSelectionControllerOnObjectSelected()
		{
			ObjectSelected = true;
		}

		private void OnEnable()
        {
	        enabled = true;
	        GameStateMachine.Instance.PinObject.SetActive(true);

	        UserInterfaceHelper.UserInterface?.SetActive(true);

			PinMovementController.PinRenderer.enabled = false;

			//PreviewObject.SetActive(true);

			ZoomInOutText = GameStateMachine.Instance.PinObject.GetComponentInChildren<TMP_Text>();
			ZoomInOutText?.gameObject?.SetActive(false);
        }

        private void OnDisable()
        {
			enabled = false;

			GameStateMachine.Instance.PinObject.SetActive(false);

			UserInterfaceHelper.UserInterface?.SetActive(false);
			ZoomInOutText?.gameObject?.SetActive(true);

			PinMovementController.PinRenderer.enabled = true;

			PreviewObject?.SetActive(false);
        }


        private void Update()
        {
	        Time.timeScale = OptionsMenuShown || ObjectSelectionShown ? 0.0f : 1.0f;

	        UpdateGroundLevel();

	        Player player = PlayerController.Instance.inputController.player;


	        if (OptionsMenuShown)
	        {
		        if (player.GetButtonDown("Select"))
		        {
			        OptionsMenuGameObject.SetActive(true);
			        OptionsMenuShown = !OptionsMenuShown;
		        }

		        return;
	        }
			if (ObjectSelectionShown)
	        {
		        if (player.GetButtonDown("Start"))
		        {
			        ObjectSelectionGameObject.SetActive(false);
			        ObjectSelectionShown = !ObjectSelectionShown;
		        }

		        return;
			}

			if (player.GetButtonSinglePressHold("LB"))
			{
				Time.timeScale = 0.0f;

				// If left stick movement, rotate the object on X/z axis
				Vector2 leftStick = player.GetAxis2D("LeftStickX", "LeftStickY");
				PreviewObject.transform.Rotate(leftStick.y, leftStick.x, 0);

				// If right stick Y movement, scale object
				var scaleFactor = 10f;
				Vector2 rightStick = player.GetAxis2D("RightStickX", "RightStickY");
				PreviewObject.transform.localScale += new Vector3(rightStick.y / scaleFactor, rightStick.y / scaleFactor, rightStick.y / scaleFactor);


				// If a, place the object, but keep the preview object
				if (player.GetButtonDown("A"))
				{
					Debug.Log("XLObjectDropper: Holding LB Pressed A");

					UISounds.Instance?.PlayOneShotSelectMajor();

					var newObject = Instantiate(PreviewObject, PreviewObject.transform.position, PreviewObject.transform.rotation);
					newObject.SetActive(true);
					SpawnedObjects.Add(newObject);

					newObject.transform.ChangeLayersRecursively("Default");
				}
				else if (player.GetButtonDown("Left Stick Button"))
				{
					PreviewObject.transform.rotation = GameStateMachine.Instance.PinObject.transform.rotation;
				}
				else if (player.GetButtonDown("Right Stick Button"))
				{
					PreviewObject.transform.localScale = Vector3.one;
				}
			}
			else
	        {
				// If dpad up/down, move object up/down
				if (PreviewObject != null)
				{
					var scaleFactor = 10f;
					Vector2 dpad = player.GetAxis2D("DPadX", "DPadY");
					PreviewObject.transform.position = new Vector3(PreviewObject.transform.position.x, PreviewObject.transform.position.y + dpad.y / scaleFactor, PreviewObject.transform.position.z);
				}

				if (player.GetButtonDown("A") && PreviewObject != null && PreviewObject.activeSelf)
				{
					// If a, place object and delete preview
					Debug.Log("XLObjectDropper: Pressed A");
					UISounds.Instance?.PlayOneShotSelectMajor();

					var newObject = Instantiate(PreviewObject, PreviewObject.transform.position, PreviewObject.transform.rotation);
					newObject.SetActive(true);

					newObject.transform.ChangeLayersRecursively("Default");

					SpawnedObjects.Add(newObject);

					PreviewObject.SetActive(false);
				}
				else if (player.GetButtonDown("X"))
				{
					// if x, open new object selection menu
					Debug.Log("XLObjectDropper: Pressed X");
					UISounds.Instance?.PlayOneShotSelectMinor();

					GameStateMachine.Instance.RequestTransitionTo(typeof(ObjectSelectionState));
				}
				else if (player.GetButtonDown("Y"))
		        {
			        counter++;

			        if (counter >= AssetBundleHelper.LoadedAssets.Count) counter = 0;

					PreviewObject.SetActive(false);
					Object.Destroy(PreviewObject);

					InstantiatePreviewObject();

					PreviewObject.SetActive(true);

					// if y, delete highlighted object (if any)
					Debug.Log("XLObjectDropper: Pressed Y");
				}
				else if (player.GetButtonDown("Left Stick Button") && PreviewObject != null)
		        {
					PreviewObject.transform.localScale = Vector3.one;
					PreviewObject.transform.rotation = GameStateMachine.Instance.PinObject.transform.rotation;
				}
				else if (player.GetButtonDown("Select"))
		        {
			        OptionsMenuShown = !OptionsMenuShown;
					OptionsMenuGameObject.SetActive(OptionsMenuShown);
		        }
				else if (player.GetButtonDown("Start"))
		        {
					ObjectSelectionShown = !ObjectSelectionShown;
					ObjectSelectionGameObject.SetActive(ObjectSelectionShown);
				}
	        }
        }

        private void InstantiatePreviewObject()
        {
	        PreviewObject = Instantiate(AssetBundleHelper.LoadedAssets.ElementAt(counter), PinMovementController.GroundIndicator.transform);
	        PinMovementController.GroundIndicator.transform.localScale = Vector3.one;
	        PreviewObject.transform.rotation = GameStateMachine.Instance.PinObject.transform.rotation;
	        PreviewObject.transform.position = GameStateMachine.Instance.PinObject.transform.position;
	        PreviewObject.transform.ChangeLayersRecursively("Ignore Raycast");
	        
		}

        private float groundLevel;

        private bool UpdateGroundLevel()
        {
	        Ray ray1 = new Ray(this.transform.position, Vector3.down);
	        Ray ray2 = new Ray(this.transform.position, Vector3.down);
	        RaycastHit raycastHit = new RaycastHit();
	        ref RaycastHit local = ref raycastHit;
	        if (!Physics.SphereCast(ray1, 0.2f, out local))
		        return false;
	        this.groundLevel = raycastHit.point.y;

			//Debug.Log("XLObjectDropper: groundLevel : " + groundLevel);

	        return true;
        }
	}

	public static class Extensions
	{
		public static void ChangeLayersRecursively(this Transform trans, string name)
		{
			trans.gameObject.layer = LayerMask.NameToLayer(name);
			foreach (Transform child in trans)
			{
				child.ChangeLayersRecursively(name);
			}
		}
	}
}