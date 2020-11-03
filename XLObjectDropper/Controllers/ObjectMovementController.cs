using GameManagement;
using Rewired;
using UnityEngine;
using UnityEngine.Animations;
using XLObjectDropper.EventStack.Events;
using XLObjectDropper.GameManagement;
using XLObjectDropper.UI;
using XLObjectDropper.UserInterface;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Controllers
{
	public class ObjectMovementController : MonoBehaviour
	{
		#region Fields
		public static ObjectMovementController Instance { get; set; }
		public static ObjectPlacementUI MovementUI { get; set; }

		public GameObject SelectedObject;
		public bool SelectedObjectActive => SelectedObject != null && SelectedObject.activeInHierarchy;

		public GameObject HighlightedObject;
		private bool HighlightedObjectActive => HighlightedObject != null && HighlightedObject.activeInHierarchy;

		private float defaultHeight = 2.5f; // originally 1.8 in pin dropper
		public float minHeight = 0.0f;
		public float maxHeight = 15f;
		public float targetHeight;
		private float currentHeight;

		private float MaxGroundAngle = 70f;
		public float groundLevel;
		public Vector3 groundNormal;
		private bool hasGround;

		private float HorizontalAcceleration = 15f;
		private float MaxCameraAcceleration = 20f;
		private float heightChangeSpeed = 2f;
		private float VerticalAcceleration = 20f;
		private float CameraRotateSpeed = 100f;
		private float MoveSpeed = 15f;
		private float lastVerticalVelocity;
		private float lastCameraVelocity;
		public float currentMoveSpeed;
		private float zoomSpeed = 10f;

		private readonly AnimationCurve HeightToHeightChangeSpeedCurve = AnimationCurve.Linear(1f, 1f, 15f, 15f);

		private Camera mainCam;
		public Transform cameraPivot;
		private Transform cameraNode;
		private Transform originalCameraNodeParent;
		private float currentCameraDist;
		private float defaultDistance = 5.0f;
		private float minDistance = 2.5f;
		private float maxDistance = 25f;
		private float originalNearClipDist;

		public CharacterController characterController;
		public CollisionFlags collisionFlags;

		private LayerMask layerMask = new LayerMask { value = 1118209 };

		private float targetDistance;
		private float rotationAngleX;
		private float rotationAngleY;

		public GridOverlayController GridOverlay;

		private bool LockCameraMovement { get; set; }

		public bool SelectingObjectFromMenu;
		private bool existingObject;
		#endregion

		private void Awake()
		{
			Instance = this;

			gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

			mainCam = Camera.main;

			cameraPivot = transform;
			cameraNode = mainCam.transform;
			originalCameraNodeParent = cameraNode.parent;
			cameraNode.SetParent(cameraPivot);

			CreateCharacterController();

			UserInterfaceHelper.UserInterface.SetActive(true);
			
			LockCameraMovement = false;

			originalNearClipDist = mainCam.nearClipPlane;
			mainCam.nearClipPlane = 0.01f;
			targetHeight = defaultHeight;

			Vector3 vector3_1 = PlayerController.Instance.skaterController.skaterRigidbody.position;
			transform.rotation = Quaternion.Euler(0.0f, 20.0f, 0.0f);
			transform.position = vector3_1;

			UpdateGroundLevel();

			if (hasGround)
			{
				vector3_1.y = groundLevel + targetHeight;
				targetHeight = Settings.Instance.GroundTracking ? defaultHeight : vector3_1.y;
			}

			transform.position = vector3_1;
			MoveCamera(true);

			cameraPivot = transform;
			cameraNode = mainCam.transform;
			cameraNode.SetParent(cameraPivot);

			rotationAngleX = cameraPivot.eulerAngles.x;
			rotationAngleY = cameraPivot.eulerAngles.y;

			targetDistance = defaultDistance;

			GridOverlay = gameObject.AddComponent<GridOverlayController>();
			GridOverlay.enabled = Settings.Instance.ShowGrid;

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
			characterController.minMoveDistance = 0.00001f;
			characterController.radius = 0.25f;
			characterController.skinWidth = 0.001f;
			characterController.slopeLimit = 80f;
			characterController.stepOffset = 0.01f;
			characterController.enabled = true;
		}

		private void OnEnable()
        {
	        LockCameraMovement = false;

	        MovementUI.GroundTracking = Settings.Instance.GroundTracking;

	        MovementUI.RotateAndScaleModeUI.GetComponent<RotationAndScaleUI>().ScalingMode = Settings.Instance.ScalingMode;
			MovementUI.RotateAndScaleModeUI.GetComponent<RotationAndScaleUI>().ScaleSnappingMode = Settings.Instance.ScaleSnappingMode;
			MovementUI.RotateAndScaleModeUI.GetComponent<RotationAndScaleUI>().RotationSnappingMode = Settings.Instance.RotationSnappingMode;

			MovementUI.SnappingModeUI.GetComponent<SnappingModeUI>().MovementSnappingMode = Settings.Instance.MovementSnappingMode;
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
	        }

	        if (GridOverlay != null)
	        {
		        GridOverlay.enabled = false;
	        }

	        if (HighlightedObject != null)
	        {
		        HighlightedObject.transform.ChangeLayersRecursively(HighlightedObject.GetSpawnableFromSpawned().PrefabLayerInfo);
		        HighlightedObject = null;
	        }

	        cameraNode.SetParent(originalCameraNodeParent, false);
		}

        private ObjectScaleAndRotateEvent ScaleAndRotateEvent;

		private void Update()
        {
	        Player player = PlayerController.Instance.inputController.player;

	        HandleObjectHighlight();

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

			// "Pause" this controller while the rotation and scale menu or the snapping mode menu is open.
			if ((player.GetButton("LB") || player.GetButton("RB")) && SelectedObjectActive) return;


			HandleLeftStick(player);
			HandleRightStick(player);
			
			HandleTriggers(player);

			MoveCamera();

			if (SelectedObjectActive)
			{
				if (player.GetButtonTimedPressUp("A", 0.0f, 0.7f)) // tap
				{
					// aids in the double tap case when selecting an object from the menu
					if (SelectingObjectFromMenu)
					{
						SelectingObjectFromMenu = false;
						return;
					}

					PlaceObject();
				}
				else if (player.GetButtonTimedPressDown("A", 0.7f)) //press
				{
					PlaceObject(false);
				}

				if (player.GetButtonDown("X")) EditObject();
				if (player.GetButtonDown("B")) Destroy(SelectedObject);
				if (player.GetButtonDown("Left Stick Button")) ResetObject();
			}
			else if (HighlightedObject != null)
			{
				if (player.GetButtonTimedPressUp("A", 0.0f, 0.7f)) // tap
				{
					SelectObject();
				}
				else if (player.GetButtonTimedPressDown("A", 0.7f)) //press
				{
					DuplicateObject();
				}

				if (player.GetButtonDown("X")) EditObject();
				if (player.GetButtonDown("Y")) DeleteObject();
				if (player.GetButtonDown("B")) GameStateMachine.Instance.RequestPauseState();
			}
			else
			{
				if (player.GetButtonDown("B")) GameStateMachine.Instance.RequestPauseState();
			}

			HandleDPadHeightAdjustment(player);
			if (player.GetButtonDown("DPadX")) ToggleLockCameraMovement();
			if (player.GetNegativeButtonDown("DPadX")) ToggleGroundTracking();
			if (player.GetButtonDown("Right Stick Button")) ResetCamera();
        }

        private void LateUpdate()
        {
	        UpdateGroundLevel();
        }

		#region Sticks
		/// <summary>
		/// Object movement
		/// </summary>
		private void HandleLeftStick(Player player)
        {
	        var leftStick = player.GetAxis2D("LeftStickX", "LeftStickY");

	        var maxMoveSpeed = Settings.Instance.Sensitivity == 0.0f ? 1.0f : MoveSpeed * Settings.Instance.Sensitivity;

			currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, maxMoveSpeed, HorizontalAcceleration * Time.deltaTime);
			var direction = cameraPivot.transform.rotation * new Vector3(leftStick.x, 0.0f, leftStick.y) * currentMoveSpeed * Time.deltaTime;
			collisionFlags = characterController.Move(new Vector3(direction.x, 0.0f, direction.z));
        }

		/// <summary>
		/// Camera movement
		/// </summary>
		private void HandleRightStick(Player player)
        {
			//TODO: Something about this new rotation method fucks up the default angle of the object dropper
			if (!LockCameraMovement)
			{
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
			}

			var maxAngle = 85f;

			rotationAngleY = ClampAngle(rotationAngleY, -maxAngle, maxAngle);

	        var rotation = Quaternion.Euler(rotationAngleY, rotationAngleX, 0);

	        Vector3 negDistance = new Vector3(0, 0, -currentCameraDist);

	        var position = rotation * negDistance + Vector3.zero;

	        cameraPivot.rotation = rotation;
	        cameraNode.rotation = rotation;
	        cameraNode.position = position;

		    if (SelectedObject != null)
	        {
		        SelectedObject.transform.position = cameraPivot.position;
	        }
		}
		#endregion

		private void HandleTriggers(Player player)
		{
			float triggers = (player.GetAxis("RT") - player.GetAxis("LT")) * Time.deltaTime * zoomSpeed; //* HeightToHeightChangeSpeedCurve.Evaluate(targetHeight);

			if (!Mathf.Approximately(triggers, 0.0f))
			{
				if ((double) currentCameraDist < (double) maxDistance && (double) triggers > 0.0 ||
				    (double) currentCameraDist > (double) minDistance && (double) triggers < 0.0)
				{
					targetDistance += triggers;
				}
			}
		}

		#region Camera movement methods
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

        private void ToggleLockCameraMovement()
        {
	        UISounds.Instance?.PlayOneShotSelectionChange();
	        LockCameraMovement = !LockCameraMovement;
		}

        private void ToggleGroundTracking()
        {
	        UISounds.Instance?.PlayOneShotSelectionChange();
	        Settings.Instance.GroundTracking = !Settings.Instance.GroundTracking;

	        Settings.Instance.Save();

	        currentHeight = transform.position.y - (Settings.Instance.GroundTracking ? groundLevel : 0.0f);
	        targetHeight = currentHeight;
        }

        private void ResetCamera()
        {
	        UISounds.Instance?.PlayOneShotSelectMajor();

	        targetDistance = defaultDistance;
	        rotationAngleX = 0;
	        rotationAngleY = 20f;
	        MoveCamera(true);
		}
		#endregion

		public void HandleDPadHeightAdjustment(Player player)
		{
			float dpad = player.GetAxis("DPadY");
			targetHeight = targetHeight + (dpad * Time.deltaTime * heightChangeSpeed * HeightToHeightChangeSpeedCurve.Evaluate(targetHeight));

			MoveObjectOnYAxis();

			UpdateSelectedObjectPosition();
		}

		public void MoveObjectOnYAxis()
		{
			currentHeight = transform.position.y - (Settings.Instance.GroundTracking ? groundLevel : 0.0f);

			float num = (targetHeight - currentHeight) / 0.25f;

			var speed =
				(Mathf.Approximately(lastVerticalVelocity, 0.0f) || (double)Mathf.Sign(num) == (double)Mathf.Sign(lastVerticalVelocity) ?
					((double)Mathf.Abs(num) <= (double)Mathf.Abs(lastVerticalVelocity)
						? num
						: Mathf.MoveTowards(lastVerticalVelocity, num, VerticalAcceleration * Time.fixedDeltaTime))
					: 0.0f);

			var motion = speed * Time.deltaTime * Vector3.up;

			collisionFlags = characterController.Move(motion);
			lastVerticalVelocity = characterController.velocity.y;

			currentHeight = transform.position.y - (Settings.Instance.GroundTracking ? groundLevel : 0.0f);
		}

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

		#region Object creation, deletion, duplication, highlight methods
		public void InstantiateSelectedObject(Spawnable spawnable)
		{
			DestroySelectedObject();

			SelectedObject = Instantiate(spawnable.Prefab);
			SelectedObject.name = spawnable.Prefab.name;

			SelectedObject.transform.ChangeLayersRecursively("Ignore Raycast");

			SelectedObject.transform.position = transform.position;
			SelectedObject.transform.rotation = spawnable.Prefab.transform.rotation;

			UserInterfaceHelper.CustomPassVolume.enabled = true;
		}

		private void PlaceObject(bool disablePreview = true)
		{
			UISounds.Instance?.PlayOneShotSelectMajor();

			var newObject = Instantiate(SelectedObject, SelectedObject.transform.position, SelectedObject.transform.rotation);
			newObject.SetActive(true);

			var spawnable = SelectedObject.GetSpawnable();
			newObject.transform.ChangeLayersRecursively(spawnable.PrefabLayerInfo);

			if (existingObject)
			{
				existingObject = false;
				spawnable = SelectedObject.GetSpawnableFromSpawned();
				spawnable.SpawnedInstance = newObject;
			}
			else
			{
				SpawnableManager.SpawnedObjects.Add(new Spawnable(spawnable, newObject));
			}

			var objPlaceEvent = new ObjectPlacedEvent(SelectedObject, newObject);
			objPlaceEvent.AddToUndoStack();

			var aimConstraint = newObject.GetComponentInChildren<AimConstraint>(true);
			var aimTarget = newObject.transform.Find("Target");
			if (aimConstraint != null && aimTarget != null)
			{
				aimTarget.gameObject.AddComponent<AimConstraintTargetController>();
			}

			//TODO: Come back to this!!
			//if (newObject.GetComponentInChildren<Rigidbody>(true) != null)
			//{
			//	ReplayRecorder.Instance.RecordedTransforms.Add(newObject.transform);

			//	var traverseReplayRecorder = Traverse.Create(ReplayRecorder.Instance);
			//	traverseReplayRecorder.Field("transformsWithRigidbody").SetValue(ReplayRecorder.Instance
			//		.RecordedTransforms
			//		.Select((t, i) =>
			//		   new Tuple<Transform, int>(t, i))
			//		.Where(t =>
			//		   t.Item1.GetComponent<Rigidbody>() != null)
			//		.Select(t => t.Item2)
			//		.ToArray());

			//	traverseReplayRecorder.Field("cachedPositions").SetValue(new Vector3[traverseReplayRecorder
			//		.Field("transformsWithRigidbody").GetValue<int[]>().Length]);

			//	traverseReplayRecorder.Field("cachedRotations").SetValue(new Quaternion[traverseReplayRecorder
			//		.Field("transformsWithRigidbody").GetValue<int[]>().Length]);

			//	ReplayEditorController.Instance.playbackController.playbackTransforms.Add(newObject.transform);
			//}

			if (disablePreview)
			{
				DestroySelectedObject();
				UserInterfaceHelper.CustomPassVolume.enabled = false;
			}
		}

		private void DuplicateObject()
		{
			if (!HighlightedObjectActive) return;

			UISounds.Instance?.PlayOneShotSelectMajor();

			var spawnable = HighlightedObject.GetSpawnable();
			if (spawnable != null)
			{
				InstantiateSelectedObject(spawnable);
			}
		}

		public void UpdateSelectedObjectPosition()
		{
			if (SelectedObjectActive)
			{
				SelectedObject.transform.position = cameraPivot.position;
			}
		}

		private void ResetObject()
		{
			UISounds.Instance?.PlayOneShotSelectMajor();
			var prefab = SelectedObject.GetPrefab();

			SelectedObject.transform.localScale = prefab != null ? prefab.transform.localScale : Vector3.one;
			SelectedObject.transform.rotation = prefab != null ? prefab.transform.rotation : Quaternion.identity;
		}

		private void DeleteObject()
		{
			var objDeletedEvent = new ObjectDeletedEvent(HighlightedObject.GetPrefab(), HighlightedObject);
			objDeletedEvent.AddToUndoStack();

			var spawnable = HighlightedObject.GetSpawnableFromSpawned();
			if (spawnable != null)
			{
				SpawnableManager.SpawnedObjects.Remove(spawnable);
			}

			UISounds.Instance?.PlayOneShotSelectMajor();
			DestroyImmediate(HighlightedObject);
		}

		private void SelectObject()
		{
			UISounds.Instance?.PlayOneShotSelectMajor();
			SelectedObject = HighlightedObject;
			existingObject = true;
			UserInterfaceHelper.CustomPassVolume.enabled = true;
		}

		private void HandleObjectHighlight()
		{
			if (HighlightedObject != null)
			{
				var spawnable = HighlightedObject.GetSpawnableFromSpawned();

				if (spawnable != null)
				{
					HighlightedObject.transform.ChangeLayersRecursively(spawnable.PrefabLayerInfo);
				}
				HighlightedObject = null;
			}

			if (!SelectedObjectActive)
			{
				if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out RaycastHit hit, Mathf.Abs(currentCameraDist) + 10.0f))
				{
					var parent = hit.transform.GetTopMostParent();

					if (hit.collider != null && parent != null)
					{
						HighlightedObject = parent.gameObject;

						HighlightedObject.transform.ChangeLayersRecursively("Ignore Raycast");
						UserInterfaceHelper.CustomPassVolume.enabled = true;
					}
				}
			}
		}

		private void DestroySelectedObject()
		{
			if (SelectedObjectActive)
			{
				SelectedObject.SetActive(false);
				DestroyImmediate(SelectedObject);
				SelectedObject = null;
			}
		}

		private void EditObject()
		{
			UISounds.Instance?.PlayOneShotSelectMajor();
		}
		#endregion
	}
}