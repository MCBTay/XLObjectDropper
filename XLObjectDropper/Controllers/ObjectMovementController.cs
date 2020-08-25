using GameManagement;
using Rewired;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using XLObjectDropper.GameManagement;
using XLObjectDropper.UserInterface;

namespace XLObjectDropper.Controllers
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
		private static Spawnable SelectedObject { get; set; }

		private static GameObject ObjectSelectionMenuGameObject;
		private static ObjectSelectionController ObjectSelectionController { get; set; }

		public static ObjectMovementController Instance { get; set; }

		private static int CurrentScaleMode { get; set; }

		private void Awake()
		{
			Instance = this;

	        SpawnedObjects = new List<GameObject>();

	        PinMovementController = GameStateMachine.Instance.PinObject.GetComponent<PinMovementController>();

	        UserInterfaceHelper.LoadUserInterface();

			OptionsMenuGameObject = new GameObject();
			OptionsMenuController = OptionsMenuGameObject.AddComponent<OptionsMenuController>();

			ObjectSelectionMenuGameObject = new GameObject();
			ObjectSelectionController = OptionsMenuGameObject.AddComponent<ObjectSelectionController>();
			ObjectSelectionController.ObjectSelected += ObjectSelectionControllerOnObjectSelected;

			CurrentScaleMode = (int)Enumerations.ScalingMode.Uniform;
	        
	        OriginalPinObject = GameStateMachine.Instance.PinObject;

	        if (!(GameStateMachine.Instance.CurrentState.GetType() != typeof(ObjectMovementState)))
				return;

			enabled = false;
        }

		private void ObjectSelectionControllerOnObjectSelected(Spawnable spawnable)
		{
			SelectedObject = spawnable;
		}

		private void OnEnable()
        {
	        enabled = true;
	        GameStateMachine.Instance.PinObject.SetActive(true);

	        UserInterfaceHelper.UserInterface?.SetActive(true);

			PinMovementController.PinRenderer.enabled = false;

			CurrentScaleMode = (int)Enumerations.ScalingMode.Uniform;

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

        private int originalLayer = -1;

        private void Update()
        {
	        Time.timeScale = OptionsMenuShown || ObjectSelectionShown ? 0.0f : 1.0f;

	        UpdateGroundLevel();

	        Player player = PlayerController.Instance.inputController.player;

	        if (SelectedObject != null)
	        {
		        Time.timeScale = 1.0f;
		        ObjectSelectionShown = false;
		        originalLayer = SelectedObject.OriginalLayer;
		        SelectedObject = null;
		        return;
	        }
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
			        ObjectSelectionMenuGameObject.SetActive(false);
			        ObjectSelectionShown = !ObjectSelectionShown;
		        }

		        return;
	        }

			if (player.GetButtonSinglePressHold("LB"))
			{
				Time.timeScale = 0.0f;

				HandleScaleModeSwitching(player);
				HandleRotation(player);
				HandleScaling(player);
				
				// If a, place the object, but keep the preview object
				if (player.GetButtonDown("A"))
				{
					PlaceObject(false);
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

					PlaceObject();
				}
				
				if (player.GetButtonDown("X"))
				{
					// if x, open new object selection menu
					Debug.Log("XLObjectDropper: Pressed X");
					UISounds.Instance?.PlayOneShotSelectMinor();

					GameStateMachine.Instance.RequestTransitionTo(typeof(ObjectSelectionState));
				}
				
				if (player.GetButtonDown("Y"))
		        {
					// if y, delete highlighted object (if any)
					Debug.Log("XLObjectDropper: Pressed Y");
				}
				
				if (player.GetButtonDown("Left Stick Button") && PreviewObject != null)
		        {
					PreviewObject.transform.localScale = Vector3.one;
					PreviewObject.transform.rotation = GameStateMachine.Instance.PinObject.transform.rotation;
				}
				
				if (player.GetButtonDown("Select"))
		        {
			        OptionsMenuShown = !OptionsMenuShown;
					OptionsMenuGameObject.SetActive(OptionsMenuShown);
		        }
				if (player.GetButtonDown("Start"))
		        {
					ObjectSelectionShown = !ObjectSelectionShown;
					ObjectSelectionMenuGameObject.SetActive(ObjectSelectionShown);
				}
	        }
        }

        private void PlaceObject(bool disablePreview = true)
        {
	        var newObject = Instantiate(PreviewObject, PreviewObject.transform.position, PreviewObject.transform.rotation);
	        newObject.SetActive(true);

	        newObject.transform.ChangeLayersRecursively(originalLayer);

	        SpawnedObjects.Add(newObject);

	        if (disablePreview)
	        {
		        PreviewObject.SetActive(false);
		        originalLayer = -1;
	        }
        }

        private void HandleRotation(Player player)
        {
			Vector2 leftStick = player.GetAxis2D("LeftStickX", "LeftStickY");
	        PreviewObject.transform.Rotate(leftStick.y, leftStick.x, 0);
        }

        private void HandleScaling(Player player)
        {
	        var scaleFactor = 15f;
	        if (!Mathf.Approximately(Settings.Instance.Sensitivity, 1)) scaleFactor *= Settings.Instance.Sensitivity;
		    else scaleFactor = 1;

	        Vector2 rightStick = player.GetAxis2D("RightStickX", "RightStickY");
	        var scale = rightStick.y / scaleFactor;

	        switch (CurrentScaleMode)
	        {
		        case (int)Enumerations.ScalingMode.Uniform:
			        PreviewObject.transform.localScale += new Vector3(scale, scale, scale);
			        break;
		        case (int)Enumerations.ScalingMode.Width:
			        PreviewObject.transform.localScale += new Vector3(scale, 0, 0);
			        break;
		        case (int)Enumerations.ScalingMode.Height:
			        PreviewObject.transform.localScale += new Vector3(0, scale, 0);
			        break;
		        case (int)Enumerations.ScalingMode.Depth:
			        PreviewObject.transform.localScale += new Vector3(0, 0, scale);
			        break;
	        }
		}

        private void HandleScaleModeSwitching(Player player)
        {
	        if (player.GetButtonDown("DPadX"))
	        {
		        CurrentScaleMode++;

		        if (CurrentScaleMode > Enum.GetValues(typeof(Enumerations.ScalingMode)).Length - 1)
			        CurrentScaleMode = 0;
	        }

	        if (player.GetNegativeButtonDown("DPadX"))
	        {
		        CurrentScaleMode--;

		        if (CurrentScaleMode < 0)
			        CurrentScaleMode = Enum.GetValues(typeof(Enumerations.ScalingMode)).Length - 1;
	        }
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

		public static void ChangeLayersRecursively(this Transform trans, int layer)
		{
			trans.gameObject.layer = layer;
			foreach (Transform child in trans)
			{
				child.ChangeLayersRecursively(layer);
			}
		}
	}
}