using GameManagement;
using Rewired;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;
using XLObjectDropper.EventStack.Events;
using XLObjectDropper.GameManagement;
using XLObjectDropper.UI;
using XLObjectDropper.UserInterface;
using XLObjectDropper.Utilities;
using Object = UnityEngine.Object;

namespace XLObjectDropper.Controllers
{
	public class ObjectMovementController : MonoBehaviour
	{
		#region Fields
		public static ObjectMovementController Instance { get; set; }
		public static ObjectPlacementUI MovementUI { get; set; }

		public GameObject SelectedObject { get; set; }
		private LayerInfo SelectedObjectLayerInfo;

		private GameObject HighlightedObject;
		private LayerInfo HighlightedObjectLayerInfo;
		
		public List<GameObject> SpawnedObjects { get; set; }

		private float defaultHeight = 2.5f; // originally 1.8 in pin dropper
		public float minHeight = 0.0f;
		public float maxHeight = 15f;
		private float targetHeight;
		private float currentHeight;

		private float MaxGroundAngle = 70f;
		private float groundLevel;
		private Vector3 groundNormal;
		private bool hasGround;

		private float HorizontalAcceleration = 10f;
		private float MaxCameraAcceleration = 5f;
		private float heightChangeSpeed = 2f;
		private float VerticalAcceleration = 20f;
		private float CameraRotateSpeed = 100f;
		private float ObjectRotateSpeed = 10f;
		private float MoveSpeed = 10f;
		private float CameraDistMoveSpeed;
		private float lastVerticalVelocity;
		private float lastCameraVelocity;
		private float currentMoveSpeed;
		private float zoomSpeed = 10f;

		public AnimationCurve HeightToMoveSpeedFactorCurve = AnimationCurve.Linear(0.5f, 0.5f, 15f, 5f);
		public AnimationCurve HeightToHeightChangeSpeedCurve = AnimationCurve.Linear(1f, 1f, 15f, 15f);
		public AnimationCurve HeightToCameraDistCurve;

		private Camera mainCam;
		public Transform cameraPivot;
		public Transform cameraNode;
		public float CameraSphereCastRadius = 0.15f;
		private float currentCameraDist;
		private float minDistance = 2.5f;
		private float maxDistance = 25f;
		private float originalNearClipDist;

		public CharacterController characterController;
		private CollisionFlags collisionFlags;

		private LayerMask layerMask = new LayerMask { value = 1118209 };

		private float targetDistance;
		private float rotationAngleX;
		private float rotationAngleY;

		private GameObject OptionsMenuGameObject;
		private OptionsMenuController OptionsMenuController { get; set; }

		private GameObject ObjectSelectionMenuGameObject;
		private ObjectSelectionController ObjectSelectionController { get; set; }

		private int CurrentScaleMode { get; set; }
		private int CurrentRotationSnappingMode { get; set; }
		private bool LockCameraMovement { get; set; }
		#endregion

		private void Awake()
		{
			Instance = this;

			gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

			mainCam = Camera.main;

			SpawnedObjects = new List<GameObject>();

			HeightToCameraDistCurve = PlayerController.Instance.pinMover.HeightToCameraDistCurve;

			cameraPivot = transform;
			cameraNode = Instantiate(mainCam.transform, cameraPivot);

			CreateCharacterController();

			UserInterfaceHelper.UserInterface.SetActive(true);
			
			CurrentScaleMode = (int)ScalingMode.Uniform;
			

	        if (!(GameStateMachine.Instance.CurrentState.GetType() != typeof(ObjectDropperState)))
				return;

			enabled = false;
        }

		private void CreateCharacterController()
		{
			characterController = gameObject.AddComponent<CharacterController>();
			characterController.center = transform.position;
			characterController.detectCollisions = true;
			characterController.enableOverlapRecovery = true;
			characterController.height = 0.01f;
			characterController.minMoveDistance = 0.001f;
			characterController.radius = 0.25f;
			characterController.skinWidth = 0.001f;
			characterController.slopeLimit = 80f;
			characterController.stepOffset = 0.01f;
			characterController.enabled = true;
		}

		private void OnEnable()
        {
	        UserInterfaceHelper.UserInterface?.SetActive(true);

	        CurrentScaleMode = (int)ScalingMode.Uniform;
			LockCameraMovement = false;

			HeightToCameraDistCurve = PlayerController.Instance.pinMover.HeightToCameraDistCurve;

			#region from PinMovementController
			originalNearClipDist = mainCam.nearClipPlane;
			mainCam.nearClipPlane = 0.01f;
			targetHeight = defaultHeight;

			Vector3 vector3_1 = PlayerController.Instance.skaterController.skaterRigidbody.position;

			transform.rotation = Quaternion.Euler(0.0f, PlayerController.Instance.cameraController._actualCam.rotation.eulerAngles.y, 0.0f);
			transform.position = vector3_1;

			UpdateGroundLevel();

			if (hasGround)
				vector3_1.y = groundLevel + targetHeight;

			transform.position = vector3_1;
			MoveCamera(true);
			#endregion

			cameraPivot = transform;
			cameraNode = Instantiate(mainCam.transform, cameraPivot);

			rotationAngleX = cameraPivot.eulerAngles.x;
			rotationAngleY = cameraPivot.eulerAngles.y;

			targetDistance = currentCameraDist;
		}

        private void OnDisable()
        {
	        UserInterfaceHelper.UserInterface?.SetActive(false);

	        if (SelectedObject != null)
	        {
		        SelectedObject.SetActive(false);
		        DestroyImmediate(SelectedObject);

		        SelectedObjectLayerInfo = null;
	        }

	        if (HighlightedObject != null)
	        {
		        HighlightedObject.transform.ChangeLayersRecursively(HighlightedObjectLayerInfo);
		        HighlightedObjectLayerInfo = null;
		        HighlightedObject = null;
			}

	        mainCam.nearClipPlane = originalNearClipDist;
		}


        private bool OptionsMenuOpen => OptionsMenuGameObject != null && OptionsMenuGameObject.activeInHierarchy;
		private bool ObjectSelectionOpen => ObjectSelectionMenuGameObject != null && ObjectSelectionMenuGameObject.activeInHierarchy;
		private bool SelectedObjectActive => SelectedObject != null && SelectedObject.activeInHierarchy;

		private void Update()
        {
	        Time.timeScale = OptionsMenuOpen || ObjectSelectionOpen ? 0.0f : 1.0f;

	        Player player = PlayerController.Instance.inputController.player;

			// I have to do this to prevent pressing A on object selection placing the item here.  Perhaps theres
			// a better way to manage object selection being open, like a coroutine?
	        if (ItemWasSelected)
	        {
		        ItemWasSelected = false;
		        return;
	        }

	        if (OptionsMenuOpen && (player.GetButtonDown("Select") || player.GetButtonDown("B")))
	        {
		        DestroyOptionsMenu();
		        return;
	        }

	        if (ObjectSelectionOpen && (player.GetButtonDown("Start") || player.GetButtonDown("B")))
	        {
		        DestroyObjectSelection();
			    return;
	        }

			if (HighlightedObject != null)
			{
				HighlightedObject.transform.ChangeLayersRecursively(HighlightedObjectLayerInfo);
				HighlightedObject = null;
			}

			if (!SelectedObjectActive)
			{
				Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
				if (Physics.Raycast(ray, out RaycastHit hit, 5f))
				{
					var parent = hit.transform.GetTopMostParent();

					if (hit.collider != null && parent != null)
					{
						HighlightedObject = parent.gameObject;
						HighlightedObjectLayerInfo = parent.GetObjectLayers();

						HighlightedObject.transform.ChangeLayersRecursively("Ignore Raycast");
						UserInterfaceHelper.CustomPassVolume.enabled = true;
					}
				}
			}

			UpdateAXBYLabels();

		    if (player.GetButton("LB"))
			{
				HandleRotationAndScalingInput(player);
			}
			else if (player.GetButton("RB"))
			{
				HandleAxisLocking(player);
			}
			else
			{
				HandleStickAndTriggerInput(player);

				
				if (SelectedObject != null && SelectedObject.activeInHierarchy)
				{
					if (player.GetButtonDown("A"))
					{
						UISounds.Instance?.PlayOneShotSelectMajor();
						PlaceObject();
					}

					if (player.GetButtonDown("Left Stick Button"))
					{
						SelectedObject.transform.localScale = Vector3.one;
						//TODO: Come back to this, get the rotation from LoadedPrefabs
						//SelectedObject.transform.rotation = LastPrefab.transform.rotation;
					}
				}
				else if (HighlightedObject != null)
				{
					if (player.GetButtonDown("A"))
					{
						UISounds.Instance?.PlayOneShotSelectMajor();
						SelectedObject = HighlightedObject;
					}

					if (player.GetButtonDown("Y"))
					{
						//TODO: Add event here

						UISounds.Instance?.PlayOneShotSelectMajor();
						DestroyImmediate(HighlightedObject);
					}
				}

				// If dpad up/down, move object up/down
				float dpad = player.GetAxis("DPadY");
				targetHeight = targetHeight + (dpad * Time.deltaTime * heightChangeSpeed * HeightToHeightChangeSpeedCurve.Evaluate(targetHeight));

				if (player.GetButtonDown("DPadX"))
				{
					UISounds.Instance?.PlayOneShotSelectMajor();
					LockCameraMovement = !LockCameraMovement;
				}

				if (player.GetButtonDown("X"))
				{
					if (SelectedObject != null && SelectedObject.activeInHierarchy)
					{
						// if x, open new object selection menu
						UISounds.Instance?.PlayOneShotSelectMajor();
						PlaceObject(false);
					}
				}

				if (player.GetButtonDown("B"))
				{
					if (SelectedObject != null && SelectedObject.activeInHierarchy)
					{
						Destroy(SelectedObject);
					}
					else
					{
						GameStateMachine.Instance.RequestPauseState();
					}
				}
				
				if (player.GetButtonDown("Right Stick Button"))
				{
					transform.rotation = Quaternion.identity;
					targetHeight = defaultHeight;
					targetDistance = defaultHeight;
					rotationAngleX = 0;
					rotationAngleY = 0;
					MoveCamera(true);
				}
				
				if (player.GetButtonDown("Select"))
		        {
					CreateOptionsMenu();
		        }

				if (player.GetButtonDown("Start"))
		        {
			        CreateObjectSelection();
				}
	        }
        }

        private void LateUpdate()
        {
	        UpdateGroundLevel();
        }

        private void UpdateAXBYLabels()
        {
			var buttonController = MovementUI.MainScreen_UI.GetComponentInChildren<AXYBController>();
			if (buttonController != null)
			{
				buttonController.SetXButtonLabelText(SelectedObject != null && SelectedObject.activeInHierarchy ? "Duplicate" : string.Empty);
				buttonController.SetAButtonLabelText(SelectedObject != null && SelectedObject.activeInHierarchy ? "Place" : "Select");
				buttonController.SetBButtonLabelText(SelectedObject != null && SelectedObject.activeInHierarchy ? "Cancel" : "Exit");
			}
        }

        private void HandleStickAndTriggerInput(Player player)
        {
			Vector2 leftStick = player.GetAxis2D("LeftStickX", "LeftStickY");
			Vector2 rightStick = player.GetAxis2D("RightStickX", "RightStickY");

			float a = (player.GetAxis(9) - player.GetAxis(8)) * Time.deltaTime * zoomSpeed; //* HeightToHeightChangeSpeedCurve.Evaluate(targetHeight);

			currentHeight = transform.position.y - groundLevel;
			currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, MoveSpeed * HeightToMoveSpeedFactorCurve.Evaluate(targetHeight), HorizontalAcceleration * Time.deltaTime);
			collisionFlags = characterController.Move(cameraPivot.transform.rotation * new Vector3(leftStick.x, 0.0f, leftStick.y) * currentMoveSpeed * Time.deltaTime);
			currentHeight = transform.position.y - groundLevel;
			if (!Mathf.Approximately(a, 0.0f))
			{
				if ((double)currentCameraDist < (double)maxDistance && (double)a > 0.0 ||
					(double)currentCameraDist > (double)minDistance && (double)a < 0.0)
				{
					targetDistance += a;
				}

				currentHeight = transform.position.y - groundLevel;
				targetHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
			}
			else
			{
				float num = (float)(((double)targetHeight - (double)currentHeight) / 0.25);
				collisionFlags = characterController.Move((Mathf.Approximately(lastVerticalVelocity, 0.0f) || (double)Mathf.Sign(num) == (double)Mathf.Sign(lastVerticalVelocity) ? ((double)Mathf.Abs(num) <= (double)Mathf.Abs(lastVerticalVelocity) ? num : Mathf.MoveTowards(lastVerticalVelocity, num, VerticalAcceleration * Time.deltaTime)) : 0.0f) * Time.deltaTime * Vector3.up);
				lastVerticalVelocity = characterController.velocity.y;
			}

			currentHeight = transform.position.y - groundLevel;

			//TODO: Something about this new rotation method fucks up the default angle of the object dropper
			#region Camera rotation
			rotationAngleX += rightStick.x * Time.deltaTime * CameraRotateSpeed;
			rotationAngleY += rightStick.y * Time.deltaTime * CameraRotateSpeed;

			var maxAngle = 85f;

			rotationAngleY = ClampAngle(rotationAngleY, -maxAngle, maxAngle);

			var rotation = Quaternion.Euler(rotationAngleY, rotationAngleX, 0);

			Vector3 negDistance = new Vector3(0, 0, -currentCameraDist);

			var position = rotation * negDistance + Vector3.zero;

			cameraPivot.rotation = rotation;
			cameraNode.position = position;

			if (SelectedObject != null)
			{
				SelectedObject.transform.position = cameraPivot.position;
			}
			#endregion

			if (!LockCameraMovement)
			{
				MoveCamera();
			}
		}

        private void MoveCamera(bool moveInstant = false)
        {
	        Ray ray = new Ray(cameraPivot.position, -cameraPivot.forward);
	        float num1 = targetDistance;

	        if (Physics.SphereCast(ray, CameraSphereCastRadius, out RaycastHit hitInfo, num1, (int)layerMask) && (double)(num1 = Mathf.Max(0.02f, hitInfo.distance - CameraSphereCastRadius)) < (double)currentCameraDist)
		        moveInstant = true;

	        if (moveInstant)
	        {
		        lastCameraVelocity = 0.0f;
		        currentCameraDist = num1;
	        }
	        else
	        {
		        float num2 = (float)(((double)num1 - (double)currentCameraDist) / 0.25);

		        float f = Mathf.Approximately(lastCameraVelocity, 0.0f) || (double)Mathf.Sign(num2) == (double)Mathf.Sign(lastCameraVelocity) ?
			        ((double)Mathf.Abs(num2) <= (double)Mathf.Abs(lastCameraVelocity) ? num2 : Mathf.MoveTowards(lastCameraVelocity, num2, MaxCameraAcceleration * Time.deltaTime)) :
			        0.0f;

				currentCameraDist = Mathf.MoveTowards(currentCameraDist, num1, Mathf.Abs(f) * Time.deltaTime);
		        currentCameraDist = Mathf.Clamp(currentCameraDist, minDistance, maxDistance);
		        lastCameraVelocity = f;
	        }

			cameraNode.localPosition = new Vector3(0.0f, 0.0f, -currentCameraDist);
			PlayerController.Instance.cameraController.MoveCameraTo(cameraNode.position, cameraNode.rotation);
        }

		private float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360F) angle += 360F;
			if (angle > 360F) angle -= 360F;
			return Mathf.Clamp(angle, min, max);
		}

		private void PlaceObject(bool disablePreview = true)
        {
	        var newObject = Instantiate(SelectedObject, SelectedObject.transform.position, SelectedObject.transform.rotation);
	        newObject.SetActive(true);

	        newObject.transform.ChangeLayersRecursively(SelectedObjectLayerInfo);

	        SpawnedObjects.Add(newObject);

	        EventStack.EventStack.Instance.AddNewAction(new ObjectPlacedEvent(SelectedObject, newObject));

			if (disablePreview)
	        {
		        SelectedObject.SetActive(false);
		        UserInterfaceHelper.CustomPassVolume.enabled = false;
	        }
        }

		#region Rotation and Scaling (holding LB)
		private void HandleRotationAndScalingInput(Player player)
        {
	        Time.timeScale = 0.0f;

	        if (SelectedObject == null || !SelectedObject.activeInHierarchy) return;

	        HandleScaleModeSwitching(player);
	        HandleRotation(player);
	        HandleScaling(player);

			HandleRotationSnappingModeSwitching(player);

			if (player.GetButtonDown("Left Stick Button"))
	        {
		        SelectedObject.transform.rotation = transform.rotation;
	        }
	        
	        if (player.GetButtonDown("Right Stick Button"))
	        {
		        SelectedObject.transform.localScale = Vector3.one;
	        }
		}

		private void HandleScaleModeSwitching(Player player)
		{
			if (player.GetButtonDown("Y"))
			{
				UISounds.Instance?.PlayOneShotSelectMajor();

				CurrentScaleMode++;

				if (CurrentScaleMode > Enum.GetValues(typeof(ScalingMode)).Length - 1)
					CurrentScaleMode = 0;
			}
		}

		private void HandleRotation(Player player)
		{
			HandleStickRotation(player);
			HandleDPadRotation(player);
		}

		private void HandleStickRotation(Player player)
		{
			Vector2 leftStick = player.GetAxis2D("LeftStickX", "LeftStickY");

			SelectedObject?.transform.RotateAround(SelectedObject.transform.position, cameraPivot.right, leftStick.y * ObjectRotateSpeed);
			
			//TODO: In the future, we'll have a toggle for local/global rotation axis
			//SelectedObject?.transform.RotateAround(SelectedObject.transform.position, cameraPivot.up, leftStick.x * ObjectRotateSpeed);
			SelectedObject?.transform.Rotate(0, leftStick.x * ObjectRotateSpeed, 0);
		}

		private void HandleDPadRotation(Player player)
		{
			float rotationIncrement = 0.0f;

			switch (CurrentRotationSnappingMode)
			{
				case (int)RotationSnappingMode.Off:
					rotationIncrement = 0.0f;
					break;
				case (int)RotationSnappingMode.Degrees15:
					rotationIncrement = 15.0f;
					break;
				case (int)RotationSnappingMode.Degrees45:
					rotationIncrement = 45.0f;
					break;
				case (int)RotationSnappingMode.Degrees90:
					rotationIncrement = 90.0f;
					break;
			}

			if (player.GetButtonDown("DPadX"))
			{
				SelectedObject.transform.Rotate(new Vector3(0, rotationIncrement, 0));
			}

			if (player.GetNegativeButtonDown("DPadX"))
			{
				SelectedObject.transform.Rotate(new Vector3(0, -rotationIncrement, 0));
			}

			if (player.GetButtonDown("DPadY"))
			{
				SelectedObject.transform.RotateAround(SelectedObject.transform.position, cameraPivot.right, rotationIncrement);
			}

			if (player.GetNegativeButtonDown("DPadY"))
			{
				SelectedObject.transform.RotateAround(SelectedObject.transform.position, cameraPivot.right, -rotationIncrement);
			}
		}

        private void HandleScaling(Player player)
        {
	        var scaleFactor = 15f;
	     //   if (!Mathf.Approximately(Settings.Instance.Sensitivity, 1)) scaleFactor *= Settings.Instance.Sensitivity;
		    //else scaleFactor = 1;

	        Vector2 rightStick = player.GetAxis2D("RightStickX", "RightStickY");
	        var scale = rightStick.y / scaleFactor;

	        switch (CurrentScaleMode)
	        {
		        case (int)ScalingMode.Uniform:
			        SelectedObject.transform.localScale += new Vector3(scale, scale, scale);
			        break;
		        case (int)ScalingMode.Width:
			        SelectedObject.transform.localScale += new Vector3(scale, 0, 0);
			        break;
		        case (int)ScalingMode.Height:
			        SelectedObject.transform.localScale += new Vector3(0, scale, 0);
			        break;
		        case (int)ScalingMode.Depth:
			        SelectedObject.transform.localScale += new Vector3(0, 0, scale);
			        break;
	        }
		}

        private void HandleRotationSnappingModeSwitching(Player player)
        {
	        if (player.GetButtonDown("X"))
	        {
		        UISounds.Instance?.PlayOneShotSelectMajor();

				CurrentRotationSnappingMode++;

		        if (CurrentRotationSnappingMode > Enum.GetValues(typeof(RotationSnappingMode)).Length - 1)
			        CurrentRotationSnappingMode = 0;
	        }
        }
		#endregion

		#region Axis Locking (holding RB)
		private void HandleAxisLocking(Player player)
		{

		}
		#endregion

		private void UpdateGroundLevel()
		{
			Ray ray1 = new Ray(transform.position, Vector3.down);
			Ray ray2 = new Ray(transform.position, Vector3.down);
			bool flag = false;
			RaycastHit raycastHit = new RaycastHit();
			ref RaycastHit local = ref raycastHit;
			int layermask = (int)this.layerMask;
			if (Physics.Raycast(ray1, out local, 10000f, layermask))
			{
				groundLevel = raycastHit.point.y;
				groundNormal = raycastHit.normal;

				if ((double)Vector3.Angle(raycastHit.normal, Vector3.up) < (double)MaxGroundAngle)
					flag = true;
			}
			if (flag == hasGround)
				return;

			hasGround = flag;
		}

		

		public void InstantiateSelectedObject(Spawnable spawnable)
        {
			SelectedObject = Instantiate(spawnable.Prefab);
			SelectedObject.name = spawnable.Prefab.name;

			SelectedObject.transform.ChangeLayersRecursively("Ignore Raycast");

	        SelectedObject.transform.position = transform.position;
	        SelectedObject.transform.rotation = spawnable.Prefab.transform.rotation;

	        UserInterfaceHelper.CustomPassVolume.enabled = true;
        }

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
			ObjectSelectionMenuGameObject.SetActive(false);
			ObjectSelectionController.ObjectClickedEvent -= ObjectSelectionControllerOnObjectClickedEvent;

			Destroy(ObjectSelectionController);
			Destroy(ObjectSelectionMenuGameObject);
		}

		private bool ItemWasSelected;

		private void ObjectSelectionControllerOnObjectClickedEvent(Spawnable spawnable)
		{
			ItemWasSelected = true;

			InstantiateSelectedObject(spawnable);
			SelectedObjectLayerInfo = spawnable.Prefab.transform.GetObjectLayers();

			DestroyObjectSelection();
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
			OptionsMenuGameObject.SetActive(false);

			Destroy(OptionsMenuController);
			Destroy(OptionsMenuGameObject);
		}
		#endregion
	}
}