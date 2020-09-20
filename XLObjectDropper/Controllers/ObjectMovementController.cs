using GameManagement;
using Rewired;
using System;
using UnityEngine;
using XLObjectDropper.EventStack.Events;
using XLObjectDropper.GameManagement;
using XLObjectDropper.UI;
using XLObjectDropper.UI.Utilities;
using XLObjectDropper.UserInterface;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Controllers
{
	public class ObjectMovementController : MonoBehaviour
	{
		#region Fields
		public static ObjectMovementController Instance { get; set; }
		public static ObjectPlacementUI MovementUI { get; set; }

		public GameObject SelectedObject { get; set; }
		private Spawnable SelectedObjectSpawnable;
		private bool SelectedObjectActive => SelectedObject != null && SelectedObject.activeInHierarchy;
		private LayerInfo SelectedObjectLayerInfo;

		private GameObject HighlightedObject;
		private bool HighlightedObjectActive => HighlightedObject != null && HighlightedObject.activeInHierarchy;
		private LayerInfo HighlightedObjectLayerInfo;

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
		private float MaxCameraAcceleration = 20f;
		private float heightChangeSpeed = 2f;
		private float VerticalAcceleration = 20f;
		private float CameraRotateSpeed = 100f;
		private float MoveSpeed = 10f;
		private float CameraDistMoveSpeed;
		private float lastVerticalVelocity;
		private float lastCameraVelocity;
		private float currentMoveSpeed;
		private float zoomSpeed = 10f;

		public AnimationCurve HeightToMoveSpeedFactorCurve = AnimationCurve.Linear(0.0f, 0.5f, 15f, 3f);
		public AnimationCurve HeightToHeightChangeSpeedCurve = AnimationCurve.Linear(1f, 1f, 15f, 15f);
		public AnimationCurve HeightToCameraDistCurve;

		private Camera mainCam;
		public Transform cameraPivot;
		public Transform cameraNode;
		public float CameraSphereCastRadius = 0.15f;
		private float currentCameraDist;
		private float defaultDistance = 5.0f;
		private float minDistance = 2.5f;
		private float maxDistance = 25f;
		private float originalNearClipDist;

		public CharacterController characterController;
		private CollisionFlags collisionFlags;

		private LayerMask layerMask = new LayerMask { value = 1118209 };

		private float targetDistance;
		private float rotationAngleX;
		private float rotationAngleY;

		private GameObject GridOverlay;
		private bool GridOverlayActive => GridOverlay != null && GridOverlay.activeInHierarchy;

		private bool LockCameraMovement { get; set; }
		private int CurrentPlacementSnappingMode { get; set; }
		#endregion

		private void Awake()
		{
			Instance = this;

			gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

			mainCam = Camera.main;

			HeightToCameraDistCurve = PlayerController.Instance.pinMover.HeightToCameraDistCurve;

			cameraPivot = transform;
			cameraNode = Instantiate(mainCam.transform, cameraPivot);

			CreateCharacterController();

			UserInterfaceHelper.UserInterface.SetActive(true);
			
			LockCameraMovement = false;

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

			targetDistance = defaultDistance;

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
	        LockCameraMovement = false;
        }

        private void OnDisable()
        {
	        mainCam.nearClipPlane = originalNearClipDist;
		}

        public void CleanUp()
        {
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
		}

        private ObjectScaleAndRotateEvent ScaleAndRotateEvent;

		private void Update()
        {
	        Player player = PlayerController.Instance.inputController.player;

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

			MovementUI.HasHighlightedObject = HighlightedObjectActive;
			MovementUI.HasSelectedObject = SelectedObjectActive;

			if (player.GetButtonDown("LB") && SelectedObjectActive)
			{
				ScaleAndRotateEvent = new ObjectScaleAndRotateEvent(SelectedObject);
			}
			if (player.GetButtonUp("LB") && SelectedObjectActive)
			{
				ScaleAndRotateEvent.newRotation = SelectedObject.transform.rotation;
				ScaleAndRotateEvent.newLocalScale = SelectedObject.transform.localScale;
				ScaleAndRotateEvent.AddToUndoStack();

				ScaleAndRotateEvent = null;
			}

			// "Pause" this controller while the rotation and scale menu is open.
			if (player.GetButton("LB")) return;

			if (player.GetButton("RB"))
			{
				HandleAxisLocking(player);
			}
			else
			{
				HandleLeftStick(player);
				HandleRightStick(player);
				HandleTriggers(player);

				if (!LockCameraMovement)
				{
					MoveCamera();
				}

				if (SelectedObject != null && SelectedObject.activeInHierarchy)
				{
					if (player.GetButtonDown("A"))
					{
						UISounds.Instance?.PlayOneShotSelectMajor();
						PlaceObject();
					}

					if (player.GetButtonDown("Left Stick Button"))
					{
						UISounds.Instance?.PlayOneShotSelectMajor();
						var prefab = SelectedObject.GetPrefab();

						SelectedObject.transform.localScale = prefab != null ? prefab.transform.localScale : Vector3.one;
						SelectedObject.transform.rotation = prefab != null ? prefab.transform.rotation : Quaternion.identity;
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
						var objDeletedEvent = new ObjectDeletedEvent(HighlightedObject.GetPrefab(), HighlightedObject);
						objDeletedEvent.AddToUndoStack();

						UISounds.Instance?.PlayOneShotSelectMajor();
						DestroyImmediate(HighlightedObject);
					}
				}

				HandleDPadHeightAdjustment(player);

				if (player.GetButtonDown("DPadX"))
				{
					UISounds.Instance?.PlayOneShotSelectionChange();
					LockCameraMovement = !LockCameraMovement;
				}

				if (player.GetNegativeButtonDown("DPadX"))
				{
					UISounds.Instance?.PlayOneShotSelectionChange();

					CurrentPlacementSnappingMode++;

					if (CurrentPlacementSnappingMode > Enum.GetValues(typeof(PlacementSnappingMode)).Length - 1)
						CurrentPlacementSnappingMode = 0;
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
					UISounds.Instance?.PlayOneShotSelectMajor();

					targetDistance = defaultDistance;
					rotationAngleX = 0;
					rotationAngleY = 20f;
					MoveCamera(true);
				}
			}
        }

        private void LateUpdate()
        {
	        UpdateGroundLevel();
        }

        private void HandleLeftStick(Player player)
        {
	        float increment = GetCurrentPlacementSnappingModeIncrement();
	        if (increment > 0.0f)
	        {
		        var leftStick = player.GetAxis2D("LeftStickX", "LeftStickY");

		        if (player.GetButtonDown("LeftStickX"))
		        {
			        collisionFlags = characterController.Move(new Vector3(increment, 0.0f, 0.0f));
		        }

		        if (player.GetNegativeButtonDown("LeftStickX"))
		        {
			        collisionFlags = characterController.Move(new Vector3(-increment, 0.0f, 0.0f));
		        }

		        if (player.GetButtonDown("LeftStickY"))
		        {
			        collisionFlags = characterController.Move(new Vector3(0.0f, 0.0f, increment));
		        }

		        if (player.GetNegativeButtonDown("LeftStickY"))
		        {
			        collisionFlags = characterController.Move(new Vector3(0.0f, 0.0f, -increment));
		        }

		        //TODO: Potentially add some code here to snap it to the grid if it's for some reason not on it?
	        }
	        else
	        {
		        var leftStick = player.GetAxis2D("LeftStickX", "LeftStickY");
		        var direction = cameraPivot.transform.rotation * new Vector3(leftStick.x, 0.0f, leftStick.y) * currentMoveSpeed * Time.deltaTime;
		        collisionFlags = characterController.Move(new Vector3(direction.x, 0.0f, direction.z));
	        }

	        if (GridOverlayActive && Settings.Instance.ShowGrid)
	        {
		        GridOverlay.transform.position = transform.position;
	        }
		}

        private void HandleRightStick(Player player)
        {
	        //TODO: Something about this new rotation method fucks up the default angle of the object dropper

			Vector2 rightStick = player.GetAxis2D("RightStickX", "RightStickY");
	        rotationAngleX += rightStick.x * Time.deltaTime * CameraRotateSpeed;

	        if (Settings.Instance.InvertCamControl)
	        {
		        rotationAngleY -= rightStick.y * Time.deltaTime * CameraRotateSpeed;
	        }
	        else
	        {
		        rotationAngleY += rightStick.y * Time.deltaTime * CameraRotateSpeed;
	        }

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
		}

        private void HandleTriggers(Player player)
        {
	        float triggers = (player.GetAxis("RT") - player.GetAxis("LT")) * Time.deltaTime * zoomSpeed; //* HeightToHeightChangeSpeedCurve.Evaluate(targetHeight);

	        currentHeight = transform.position.y - groundLevel;
	        currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, MoveSpeed * HeightToMoveSpeedFactorCurve.Evaluate(targetHeight), HorizontalAcceleration * Time.deltaTime);

			currentHeight = transform.position.y - groundLevel;
	        if (!Mathf.Approximately(triggers, 0.0f))
	        {
		        if ((double)currentCameraDist < (double)maxDistance && (double)triggers > 0.0 ||
		            (double)currentCameraDist > (double)minDistance && (double)triggers < 0.0)
		        {
			        targetDistance += triggers;
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
		}

        public void MoveCamera(bool moveInstant = false)
        {
	        if (moveInstant)
	        {
		        lastCameraVelocity = 0.0f;
		        currentCameraDist = targetDistance;
	        }
	        else
	        {
		        float newTargetDistance = targetDistance - currentCameraDist;

		        float f = Mathf.Approximately(lastCameraVelocity, 0.0f) || (double)Mathf.Sign(newTargetDistance) == (double)Mathf.Sign(lastCameraVelocity) ?
			        ((double)Mathf.Abs(newTargetDistance) <= (double)Mathf.Abs(lastCameraVelocity) ? 
				        newTargetDistance : 
				        Mathf.MoveTowards(lastCameraVelocity, newTargetDistance, MaxCameraAcceleration * Time.deltaTime)) :
			        0.0f;

		        currentCameraDist = Mathf.MoveTowards(currentCameraDist, targetDistance, Mathf.Abs(f) * Time.deltaTime);
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

	        SpawnableManager.SpawnedObjects.Add(new Spawnable(SelectedObjectSpawnable.Prefab, newObject, SelectedObjectSpawnable.PreviewTexture));

			var objPlaceEvent = new ObjectPlacedEvent(SelectedObject, newObject);
			objPlaceEvent.AddToUndoStack();

			if (disablePreview)
	        {
		        SelectedObject.SetActive(false);
		        UserInterfaceHelper.CustomPassVolume.enabled = false;

		        if (GridOverlay != null && GridOverlay.activeInHierarchy)
		        {
					GridOverlay.SetActive(false);
					DestroyImmediate(GridOverlay);
		        }
	        }
        }

		private float GetCurrentPlacementSnappingModeIncrement()
		{
			float increment = 0.0f;

			switch (CurrentPlacementSnappingMode)
			{
				case (int)PlacementSnappingMode.Quarter:
					increment = 0.25f;
					break;
				case (int)PlacementSnappingMode.Half:
					increment = 0.5f;
					break;
				case (int)PlacementSnappingMode.Full:
					increment = 1.0f;
					break;
				case (int)PlacementSnappingMode.Double:
					increment = 2.0f;
					break;
				case (int)PlacementSnappingMode.Off:
				default:
					increment = 0.0f;
					break;
			}

			return increment;
		}

		private void HandleDPadHeightAdjustment(Player player)
		{
			var increment = GetCurrentPlacementSnappingModeIncrement();

			if (increment > 0)
			{
				if (player.GetButtonDown("DPadY"))
				{
					targetHeight += increment;
				}

				if (player.GetNegativeButtonDown("DPadY"))
				{
					targetHeight -= increment;
				}
			}
			else
			{
				// If dpad up/down, move object up/down
				float dpad = player.GetAxis("DPadY");
				targetHeight = targetHeight + (dpad * Time.deltaTime * heightChangeSpeed * HeightToHeightChangeSpeedCurve.Evaluate(targetHeight));
			}
		}

		#region Axis Locking (holding RB)
		private void HandleAxisLocking(Player player)
		{
			HandleDPadHeightAdjustment(player);

			currentHeight = transform.position.y - groundLevel;

			float num = (float)(((double)targetHeight - (double)currentHeight) / 0.25);
			collisionFlags = characterController.Move((Mathf.Approximately(lastVerticalVelocity, 0.0f) || (double)Mathf.Sign(num) == (double)Mathf.Sign(lastVerticalVelocity) ? ((double)Mathf.Abs(num) <= (double)Mathf.Abs(lastVerticalVelocity) ? num : Mathf.MoveTowards(lastVerticalVelocity, num, VerticalAcceleration * Time.deltaTime)) : 0.0f) * Time.deltaTime * Vector3.up);
			lastVerticalVelocity = characterController.velocity.y;

			currentHeight = transform.position.y - groundLevel;


			var leftStick = player.GetAxis2D("LeftStickX", "LeftStickY");

			Vector3 direction = Vector3.zero;

			if (Mathf.Abs(leftStick.x) > Mathf.Abs(leftStick.y))
			{
				direction = new Vector3(Mathf.Sign(leftStick.x), 0.0f, 0.0f);
			}
			else if (Mathf.Abs(leftStick.x) < Mathf.Abs(leftStick.y))
			{
				direction = new Vector3(0.0f, 0.0f, Mathf.Sign(leftStick.y));
			}

			var motion = direction * currentMoveSpeed * Time.deltaTime;
			collisionFlags = characterController.Move(new Vector3(motion.x, 0.0f, motion.z));

			if (SelectedObjectActive)
			{
				SelectedObject.transform.position = cameraPivot.position;

				if (GridOverlayActive && Settings.Instance.ShowGrid)
				{
					GridOverlay.transform.position = SelectedObject.transform.position;
				}
			}
		}
		#endregion

		private void UpdateGroundLevel()
		{
			Ray ray1 = new Ray(transform.position, Vector3.down);
			
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

			SelectedObjectSpawnable = spawnable;
			SelectedObjectLayerInfo = spawnable.Prefab.transform.GetObjectLayers();

			if (Settings.Instance.ShowGrid)
			{
				GridOverlay = Instantiate(AssetBundleHelper.GridOverlayPrefab);
				GridOverlay.transform.position = SelectedObject.transform.position;
			}

			UserInterfaceHelper.CustomPassVolume.enabled = true;
		}
	}
}