using GameManagement;
using Rewired;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;
using XLObjectDropper.GameManagement;
using XLObjectDropper.UI;
using XLObjectDropper.UserInterface;

namespace XLObjectDropper.Controllers
{
	public class ObjectMovementController : MonoBehaviour
	{
		public static GameObject PreviewObject { get; set; }
        public List<GameObject> SpawnedObjects { get; set; }

        private static bool OptionsMenuShown { get; set; }
		private static GameObject OptionsMenuGameObject;
		private static OptionsMenuController OptionsMenuController { get; set; }

		private static bool ObjectSelectionShown { get; set; }
		private static Spawnable SelectedObject { get; set; }

		private static GameObject ObjectSelectionMenuGameObject;
		private static ObjectSelectionController ObjectSelectionController { get; set; }

		public static ObjectMovementController Instance { get; set; }


		private float defaultHeight = 2.5f; // originally 1.8 in pin dropper
		public float HorizontalAcceleration = 10f;
		private float MaxGroundAngle = 70f;
		public float MaxCameraAcceleration = 5f;
		public AnimationCurve HeightToMoveSpeedFactorCurve = AnimationCurve.Linear(0.5f, 0.5f, 15f, 5f);
		public AnimationCurve HeightToHeightChangeSpeedCurve = AnimationCurve.Linear(1f, 1f, 15f, 15f);

		public GameObject cameraPivotGameObject;
		public Transform cameraPivot;

		public Transform cameraNode;
		public float minHeight = 0.5f;
		public float maxHeight = 15f;
		public float heightChangeSpeed = 2f;
		public float VerticalAcceleration = 20f;
		public float RotateSpeed = 100f;
		public float MoveSpeed = 10f;
		public float CameraDistMoveSpeed;
		public AnimationCurve HeightToCameraDistCurve;
		public CharacterController characterController;
		private LayerMask layermask = new LayerMask { value = 1118209 };
		private RaycastHit lastHit;
		private float groundLevel;
		private Vector3 groundNormal;
		private float targetHeight;
		private float currentCameraDist;
		private float currentHeight;
		private float originalNearClipDist;
		private Camera mainCam;
		private float lastVerticalVelocity;
		private float lastCameraVelocity;
		private CollisionFlags collisionFlags;
		public float CameraSphereCastRadius = 0.15f;
		private float currentMoveSpeed;

		private bool hasGround;

		private GameObject LastPrefab;

		private float targetDistance;
		private float rotationAngleX;
		private float rotationAngleY;

		private int CurrentScaleMode { get; set; }
		private int CurrentRotationSnappingMode { get; set; }
		public bool LockCameraMovement { get; private set; }

		private void Awake()
		{
			Instance = this;

			this.mainCam = Camera.main;

			SpawnedObjects = new List<GameObject>();

			HeightToCameraDistCurve = PlayerController.Instance.pinMover.HeightToCameraDistCurve;

			cameraPivot = transform;
			cameraNode = Instantiate(mainCam.transform, cameraPivot);

			characterController = this.gameObject.AddComponent<CharacterController>();
			characterController.center = PlayerController.Instance.pinMover.characterController.center;
			characterController.detectCollisions = PlayerController.Instance.pinMover.characterController.detectCollisions;
			characterController.enableOverlapRecovery = PlayerController.Instance.pinMover.characterController.enableOverlapRecovery;
			characterController.height = PlayerController.Instance.pinMover.characterController.height;
			//characterController.isGrounded = PlayerController.Instance.pinMover.characterController.isGrounded;
			characterController.minMoveDistance = PlayerController.Instance.pinMover.characterController.minMoveDistance;
			characterController.radius = PlayerController.Instance.pinMover.characterController.radius;
			characterController.skinWidth = PlayerController.Instance.pinMover.characterController.skinWidth;
			characterController.slopeLimit = PlayerController.Instance.pinMover.characterController.slopeLimit;
			characterController.stepOffset = PlayerController.Instance.pinMover.characterController.stepOffset;
			characterController.enabled = true;

			UserInterfaceHelper.LoadUserInterface();

			OptionsMenuGameObject = new GameObject();
			OptionsMenuController = OptionsMenuGameObject.AddComponent<OptionsMenuController>();

			CurrentScaleMode = (int)ScalingMode.Uniform;
			

	        if (!(GameStateMachine.Instance.CurrentState.GetType() != typeof(ObjectDropperState)))
				return;

			enabled = false;
        }

		#region Object Selection 
		private void CreateObjectSelection()
		{
			ObjectSelectionMenuGameObject = new GameObject();
			ObjectSelectionController = OptionsMenuGameObject.AddComponent<ObjectSelectionController>();
			ObjectSelectionController.ObjectClickedEvent += ObjectSelectionControllerOnObjectClickedEvent;

			ObjectSelectionMenuGameObject.SetActive(true);

			ObjectSelectionShown = true;
		}

		private void DestroyObjectSelection()
		{
			ObjectSelectionMenuGameObject.SetActive(false);
			ObjectSelectionController.ObjectClickedEvent -= ObjectSelectionControllerOnObjectClickedEvent;

			Destroy(ObjectSelectionController);
			Destroy(ObjectSelectionMenuGameObject);

			ObjectSelectionShown = false;
		}

		private void ObjectSelectionControllerOnObjectClickedEvent(Spawnable spawnable)
		{
			SelectedObject = spawnable;
			DestroyObjectSelection();
		}
		#endregion

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
			cameraNode = mainCam.transform;

			rotationAngleX = cameraPivot.eulerAngles.x;
			rotationAngleY = cameraPivot.eulerAngles.y;

			targetDistance = currentCameraDist;
		}

        private void OnDisable()
        {
	        UserInterfaceHelper.UserInterface?.SetActive(false);
			PreviewObject?.SetActive(false);

			mainCam.nearClipPlane = originalNearClipDist;
		}

        private void Update()
        {
	        Time.timeScale = OptionsMenuShown || ObjectSelectionShown ? 0.0f : 1.0f;

	        Player player = PlayerController.Instance.inputController.player;

	        if (SelectedObject != null)
	        {
		        Time.timeScale = 1.0f;
				DestroyObjectSelection();

				LastPrefab = SelectedObject.Prefab;
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
			        DestroyObjectSelection();
		        }

		        return;
	        }

			if (player.GetButtonSinglePressHold("LB"))
			{
				HandleRotationAndScalingInput(player);
			}
			else if (player.GetButtonSinglePressHold("RB"))
			{
				HandleAxisLocking(player);
			}
			else
			{
				HandleStickAndTriggerInput();

				
				if (PreviewObject != null && PreviewObject.activeInHierarchy)
				{
					// If dpad up/down, move object up/down
					float dpad = player.GetAxis("DPadY");
					targetHeight = targetHeight + (dpad * Time.deltaTime * heightChangeSpeed * HeightToHeightChangeSpeedCurve.Evaluate(targetHeight));

					if (player.GetButtonDown("A"))
					{
						UISounds.Instance?.PlayOneShotSelectMajor();
						PlaceObject();
					}

					if (player.GetButtonDown("Left Stick Button"))
					{
						PreviewObject.transform.localScale = Vector3.one;
						PreviewObject.transform.rotation = LastPrefab.transform.rotation;
					}
				}

				if (player.GetButtonDown("DPadX"))
				{
					UISounds.Instance?.PlayOneShotSelectMajor();
					LockCameraMovement = !LockCameraMovement;
				}

				if (player.GetButtonDown("X"))
				{
					// if x, open new object selection menu
					UISounds.Instance?.PlayOneShotSelectMinor();
				}
				
				if (player.GetButtonDown("Y"))
		        {
					// if y, delete highlighted object (if any)
		        }
				
				if (player.GetButtonDown("Right Stick Button"))
				{
					//TODO: Re-evaluate this
					//PinMovementController.transform.rotation = Quaternion.identity;
					//Traverse.Create(PinMovementController).Field("targetHeight").SetValue(PinMovementController.defaultHeight);
					//Traverse.Create(PinMovementController).Method("MoveCamera", true).GetValue();
				}
				
				if (player.GetButtonDown("Select"))
		        {
			        OptionsMenuGameObject.SetActive(OptionsMenuShown);
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

        private void HandleStickAndTriggerInput()
        {
			Vector2 leftStick = PlayerController.Instance.inputController.player.GetAxis2D("LeftStickX", "LeftStickY");
			Vector2 rightStick = PlayerController.Instance.inputController.player.GetAxis2D("RightStickX", "RightStickY");

			float a = (PlayerController.Instance.inputController.player.GetAxis(9) - PlayerController.Instance.inputController.player.GetAxis(8)) *
					  Time.deltaTime *
					  heightChangeSpeed *
					  HeightToHeightChangeSpeedCurve.Evaluate(targetHeight);

			currentHeight = transform.position.y - groundLevel;
			currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, MoveSpeed * HeightToMoveSpeedFactorCurve.Evaluate(targetHeight), HorizontalAcceleration * Time.deltaTime);
			collisionFlags = characterController.Move(transform.rotation * new Vector3(leftStick.x, 0.0f, leftStick.y) * currentMoveSpeed * Time.deltaTime);
			currentHeight = transform.position.y - groundLevel;
			if (!Mathf.Approximately(a, 0.0f))
			{
				if ((double)currentHeight < (double)maxHeight && (double)a > 0.0 ||
					(double)currentHeight > (double)minHeight && (double)a < 0.0)
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
			rotationAngleX += rightStick.x * Time.deltaTime * RotateSpeed;
			rotationAngleY += rightStick.y * Time.deltaTime * RotateSpeed;

			var maxAngle = 85f;

			rotationAngleY = ClampAngle(rotationAngleY, -maxAngle, maxAngle);

			var rotation = Quaternion.Euler(rotationAngleY, rotationAngleX, 0);

			Vector3 negDistance = new Vector3(0, 0, -currentCameraDist);

			var position = rotation * negDistance + Vector3.zero;

			cameraPivot.rotation = rotation;

			cameraNode.position = position;

			if (PreviewObject != null)
			{
				PreviewObject.transform.position = cameraPivot.position;
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
	        float num1 = HeightToCameraDistCurve.Evaluate(targetHeight);

	        if (Physics.SphereCast(ray, CameraSphereCastRadius, out RaycastHit hitInfo, num1, (int)layermask) && (double)(num1 = Mathf.Max(0.02f, hitInfo.distance - CameraSphereCastRadius)) < (double)currentCameraDist)
		        moveInstant = true;

	        if (moveInstant)
	        {
		        lastCameraVelocity = 0.0f;
		        currentCameraDist = num1;
	        }
	        else
	        {
		        float num2 = (float)(((double)targetDistance - (double)currentCameraDist) / 0.25);

		        float f = Mathf.Approximately(lastCameraVelocity, 0.0f) || (double)Mathf.Sign(num2) == (double)Mathf.Sign(lastCameraVelocity) ?
			        ((double)Mathf.Abs(num2) <= (double)Mathf.Abs(lastCameraVelocity) ? num2 : Mathf.MoveTowards(lastCameraVelocity, num2, MaxCameraAcceleration * Time.deltaTime)) :
			        0.0f;

				currentCameraDist = Mathf.MoveTowards(currentCameraDist, targetDistance, Mathf.Abs(f) * Time.deltaTime);
		        currentCameraDist = Mathf.Clamp(currentCameraDist, 2.5f, 25f);
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
	        var newObject = Instantiate(PreviewObject, PreviewObject.transform.position, PreviewObject.transform.rotation);
	        newObject.SetActive(true);

	        newObject.transform.ChangeLayersRecursively(LastPrefab);

	        SpawnedObjects.Add(newObject);

	        if (disablePreview)
	        {
		        PreviewObject.SetActive(false);
	        }
        }

		#region Rotation and Scaling (holding LB)
		private void HandleRotationAndScalingInput(Player player)
        {
	        Time.timeScale = 0.0f;

	        if (PreviewObject == null || !PreviewObject.activeInHierarchy) return;

	        HandleScaleModeSwitching(player);
	        HandleRotation(player);
	        HandleScaling(player);

	        if (player.GetButtonDown("A"))
	        {
		        PlaceObject(false);
	        }

			HandleRotationSnappingModeSwitching(player);

			if (player.GetButtonDown("Left Stick Button"))
	        {
		        PreviewObject.transform.rotation = transform.rotation;
	        }
	        
	        if (player.GetButtonDown("Right Stick Button"))
	        {
		        PreviewObject.transform.localScale = Vector3.one;
	        }
		}

		private void HandleScaleModeSwitching(Player player)
		{
			if (player.GetButtonDown("DPadX"))
			{
				CurrentScaleMode++;

				if (CurrentScaleMode > Enum.GetValues(typeof(ScalingMode)).Length - 1)
					CurrentScaleMode = 0;
			}

			if (player.GetNegativeButtonDown("DPadX"))
			{
				CurrentScaleMode--;

				if (CurrentScaleMode < 0)
					CurrentScaleMode = Enum.GetValues(typeof(ScalingMode)).Length - 1;
			}
		}

		private void HandleRotation(Player player)
        {
	        Vector2 leftStick = player.GetAxis2D("LeftStickX", "LeftStickY");

	        switch (CurrentScaleMode)
	        {
		        case (int)RotationSnappingMode.Off:

			        PreviewObject?.transform.Rotate(leftStick.y, leftStick.x, 0);
			        break;
				case (int)RotationSnappingMode.Degrees15:
					PreviewObject?.transform.Rotate(leftStick.y + 15, leftStick.x + 15, 0);
					break;
				case (int)RotationSnappingMode.Degrees45:
					PreviewObject?.transform.Rotate(leftStick.y + 45, leftStick.x + 45, 0);
					break;
				case (int)RotationSnappingMode.Degrees90:
					PreviewObject?.transform.Rotate(leftStick.y + 90, leftStick.x + 90, 0);
					break;
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
			        PreviewObject.transform.localScale += new Vector3(scale, scale, scale);
			        break;
		        case (int)ScalingMode.Width:
			        PreviewObject.transform.localScale += new Vector3(scale, 0, 0);
			        break;
		        case (int)ScalingMode.Height:
			        PreviewObject.transform.localScale += new Vector3(0, scale, 0);
			        break;
		        case (int)ScalingMode.Depth:
			        PreviewObject.transform.localScale += new Vector3(0, 0, scale);
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
			int layermask = (int)this.layermask;
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

		public void InstantiatePreviewObject(Spawnable spawnable)
        {
			PreviewObject = Instantiate(spawnable.Prefab);

			PreviewObject.transform.ChangeLayersRecursively("Ignore Raycast");

	        PreviewObject.transform.position = transform.position;
	        PreviewObject.transform.rotation = spawnable.Prefab.transform.rotation;
        }
	}
}