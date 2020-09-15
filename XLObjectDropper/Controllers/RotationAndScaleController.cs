using Rewired;
using System;
using UnityEngine;
using XLObjectDropper.UI.Utilities;

namespace XLObjectDropper.Controllers
{
	public class RotationAndScaleController : MonoBehaviour
	{
		public static RotationAndScaleController Instance;

		public GameObject SelectedObject;
		public Transform cameraPivot;

		private float ObjectRotateSpeed = 10f;

		private int CurrentScaleMode;
		private int CurrentRotationSnappingMode;

		private void Awake()
		{
			Instance = this;
		}

		private void OnEnable()
		{
			cameraPivot = ObjectMovementController.Instance.cameraPivot;
			SelectedObject = ObjectMovementController.Instance.SelectedObject;
		}

		private void Update()
		{
			Time.timeScale = 0.0f;
			Player player = PlayerController.Instance.inputController.player;

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
				UISounds.Instance?.PlayOneShotSelectionChange();

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
				UISounds.Instance?.PlayOneShotSelectionChange();

				CurrentRotationSnappingMode++;

				if (CurrentRotationSnappingMode > Enum.GetValues(typeof(RotationSnappingMode)).Length - 1)
					CurrentRotationSnappingMode = 0;
			}
		}
	}
}
