using System;
using Rewired;
using UnityEngine;
using UnityModManagerNet;
using XLObjectDropper.UI.Utilities;

namespace XLObjectDropper.Controllers
{
	public class SnappingModeController : MonoBehaviour
	{
		public static SnappingModeController Instance;

		private void Awake()
		{
			Instance = this;
		}

		private void Update()
		{
			Player player = PlayerController.Instance.inputController.player;

			if (!ObjectMovementController.Instance.SelectedObjectActive) return;

			HandleLeftStick(player);
			HandleDPadHeightAdjustment(player);

			if (player.GetButtonDown("Y")) SnapToGrid();
			if (player.GetButtonDown("X")) UpdateMovementSnappingMode();
			if (player.GetButtonDown("A")) SnapToGround();
		}

		//TODO: Control which stick axis controls which movement direction based on camera position
		private void HandleLeftStick(Player player)
		{
			var increment = GetCurrentPlacementSnappingModeIncrement();

			if (!Mathf.Approximately(increment, 0.0f))
			{
				Vector3 direction = Vector3.zero;

				if (player.GetButtonDown("LeftStickX")) direction = new Vector3(increment, 0.0f, 0.0f);
				if (player.GetNegativeButtonDown("LeftStickX")) direction = new Vector3(-increment, 0.0f, 0.0f);

				if (player.GetButtonDown("LeftStickY")) direction = new Vector3(0.0f, 0.0f, increment);
				if (player.GetNegativeButtonDown("LeftStickY")) direction = new Vector3(0.0f, 0.0f, -increment);

				ObjectMovementController.Instance.collisionFlags = ObjectMovementController.Instance.characterController.Move(direction);
			}
			else
			{
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

				var motion = direction * ObjectMovementController.Instance.currentMoveSpeed * Time.deltaTime;
				ObjectMovementController.Instance.collisionFlags = ObjectMovementController.Instance.characterController.Move(new Vector3(motion.x, 0.0f, motion.z));
			}

			ObjectMovementController.Instance.UpdateSelectedObjectPosition();
		}

		private void UpdateMovementSnappingMode()
		{
			UISounds.Instance?.PlayOneShotSelectionChange();

			Settings.Instance.MovementSnappingMode++;

			if ((int)Settings.Instance.MovementSnappingMode > Enum.GetValues(typeof(MovementSnappingMode)).Length - 1)
				Settings.Instance.MovementSnappingMode = 0;

			Settings.Instance.Save();
		}

		private float GetCurrentPlacementSnappingModeIncrement()
		{
			float increment = 0.0f;

			switch (Settings.Instance.MovementSnappingMode)
			{
				case MovementSnappingMode.Quarter:
					increment = 0.25f;
					break;
				case MovementSnappingMode.Half:
					increment = 0.5f;
					break;
				case MovementSnappingMode.Full:
					increment = 1.0f;
					break;
				case MovementSnappingMode.Double:
					increment = 2.0f;
					break;
				case MovementSnappingMode.Off:
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
					ObjectMovementController.Instance.targetHeight += increment;
				}

				if (player.GetNegativeButtonDown("DPadY"))
				{
					ObjectMovementController.Instance.targetHeight -= increment;
				}

				ObjectMovementController.Instance.MoveObjectOnYAxis();
				ObjectMovementController.Instance.UpdateSelectedObjectPosition();
			}
			else
			{
				ObjectMovementController.Instance.HandleDPadHeightAdjustment(player);
			}
		}

		private void SnapToGrid()
		{
			UISounds.Instance?.PlayOneShotSelectMajor();

			var increment = GetCurrentPlacementSnappingModeIncrement();

			if (Mathf.Approximately(increment, 0.0f)) increment = 1.0f;

			var roundedX = Mathf.Round(ObjectMovementController.Instance.cameraPivot.position.x / increment) * increment;
			var roundedY = Mathf.Round(ObjectMovementController.Instance.cameraPivot.position.y / increment) * increment;
			var roundedZ = Mathf.Round(ObjectMovementController.Instance.cameraPivot.position.z / increment) * increment;

			UnityModManager.Logger.Log("XLObjectDropper: Setting position from " + ObjectMovementController.Instance.cameraPivot.position + " to " + new Vector3(roundedX, roundedY, roundedZ));
			ObjectMovementController.Instance.cameraPivot.position = new Vector3(roundedX, roundedY, roundedZ);

			ObjectMovementController.Instance.UpdateSelectedObjectPosition();
		}

		private void SnapToGround()
		{
			UISounds.Instance?.PlayOneShotSelectMajor();

			ObjectMovementController.Instance.targetHeight = Settings.Instance.GroundTracking ? 0.0f : ObjectMovementController.Instance.groundLevel;
			ObjectMovementController.Instance.MoveObjectOnYAxis();
			ObjectMovementController.Instance.UpdateSelectedObjectPosition();

			ObjectMovementController.Instance.SelectedObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, ObjectMovementController.Instance.groundNormal);
		}
	}
}
