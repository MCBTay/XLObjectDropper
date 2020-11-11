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

			if (player.GetButtonDown("A")) HandleScaleSnappingModeSwitching();
			if (player.GetButtonDown("Y")) HandleScaleModeSwitching();
			if (player.GetButtonDown("X")) HandleRotationSnappingModeSwitching();

			HandleRotation(player);
			HandleScaling(player);

			if (player.GetButtonDown("Left Stick Button"))
			{
				UISounds.Instance?.PlayOneShotSelectMajor();
				SelectedObject.transform.rotation = transform.rotation;
			}

			if (player.GetButtonDown("Right Stick Button"))
			{
				UISounds.Instance?.PlayOneShotSelectMajor();
				SelectedObject.transform.localScale = Vector3.one;
			}
		}

		private void HandleScaleSnappingModeSwitching()
		{
			UISounds.Instance?.PlayOneShotSelectionChange();

			Settings.Instance.ScaleSnappingMode++;

			if ((int)Settings.Instance.ScaleSnappingMode > Enum.GetValues(typeof(ScaleSnappingMode)).Length - 1)
				Settings.Instance.ScaleSnappingMode = ScaleSnappingMode.Off;

			Settings.Instance.Save();
		}

		private void HandleScaleModeSwitching()
		{
			UISounds.Instance?.PlayOneShotSelectionChange();

			Settings.Instance.ScalingMode++;

			if ((int)Settings.Instance.ScalingMode > Enum.GetValues(typeof(ScalingMode)).Length - 1)
				Settings.Instance.ScalingMode = ScalingMode.Uniform;

			Settings.Instance.Save();
		}

		private void HandleRotationSnappingModeSwitching()
		{
			UISounds.Instance?.PlayOneShotSelectionChange();

			Settings.Instance.RotationSnappingMode++;

			if ((int)Settings.Instance.RotationSnappingMode > Enum.GetValues(typeof(RotationSnappingMode)).Length - 1)
				Settings.Instance.RotationSnappingMode = RotationSnappingMode.Off;

			Settings.Instance.Save();
		}

		private void HandleRotation(Player player)
		{
			HandleStickRotation(player);
			HandleDPadRotation(player);
		}

		private void HandleStickRotation(Player player)
		{
			Vector2 leftStick = player.GetAxis2D("LeftStickX", "LeftStickY");

			var maxRotateSpeed = Settings.Instance.Sensitivity == 0.0f ? 1.0f : ObjectRotateSpeed * Settings.Instance.Sensitivity;

			SelectedObject.transform.RotateAround(SelectedObject.transform.position, cameraPivot.right, leftStick.y * maxRotateSpeed);

			//TODO: In the future, we'll have a toggle for local/global rotation axis
			//SelectedObject.transform.RotateAround(SelectedObject.transform.position, cameraPivot.up, leftStick.x * maxRotateSpeed); //global
			SelectedObject.transform.Rotate(0, leftStick.x * maxRotateSpeed, 0); //local
		}

		private void HandleDPadRotation(Player player)
		{
			float rotationIncrement = 0.0f;

			switch (Settings.Instance.RotationSnappingMode)
			{
				case RotationSnappingMode.Off:
					rotationIncrement = 0.0f;
					break;
				case RotationSnappingMode.Degrees15:
					rotationIncrement = 15.0f;
					break;
				case RotationSnappingMode.Degrees45:
					rotationIncrement = 45.0f;
					break;
				case RotationSnappingMode.Degrees90:
					rotationIncrement = 90.0f;
					break;
			}

			if (Mathf.Approximately(rotationIncrement, 0.0f)) return;

			if (player.GetButtonDown("DPadX"))
			{
				UISounds.Instance?.PlayOneShotSelectionChange();

				//TODO: In the future, we'll have a toggle for local/global rotation axis
				//SelectedObject.transform.RotateAround(SelectedObject.transform.position, cameraPivot.up, rotationIncrement); //global
				SelectedObject.transform.Rotate(new Vector3(0, rotationIncrement, 0)); //local
			}

			if (player.GetNegativeButtonDown("DPadX"))
			{
				UISounds.Instance?.PlayOneShotSelectionChange();
				
				//TODO: In the future, we'll have a toggle for local/global rotation axis
				//SelectedObject.transform.RotateAround(SelectedObject.transform.position, cameraPivot.up, -rotationIncrement); //global
				SelectedObject.transform.Rotate(new Vector3(0, -rotationIncrement, 0)); //local
			}

			if (player.GetButtonDown("DPadY"))
			{
				UISounds.Instance?.PlayOneShotSelectionChange();
				SelectedObject.transform.RotateAround(SelectedObject.transform.position, cameraPivot.right, rotationIncrement);
			}

			if (player.GetNegativeButtonDown("DPadY"))
			{
				UISounds.Instance?.PlayOneShotSelectionChange();
				SelectedObject.transform.RotateAround(SelectedObject.transform.position, cameraPivot.right, -rotationIncrement);
			}
		}

		private void HandleScaling(Player player)
		{
			var scaleSpeed = 50f;
			var increment = GetScaleSnappingIncrement();

			float scaleFactor = 0.0f;

			if (Mathf.Approximately(increment, 0))
			{
				Vector2 rightStick = player.GetAxis2D("RightStickX", "RightStickY");
				
				var maxScaleSpeed = 1 - Settings.Instance.Sensitivity == 0.0f ? 1.0f : scaleSpeed * (1 - Settings.Instance.Sensitivity);
				if (maxScaleSpeed != 0)
					scaleFactor = rightStick.y / maxScaleSpeed;
			}
			else
			{
				if (player.GetButtonDown("RightStickY")) scaleFactor = increment;
				if (player.GetNegativeButtonDown("RightSTickY")) scaleFactor = -increment;
			}

			Vector3 scale = Vector3.zero;

			switch (Settings.Instance.ScalingMode)
			{
				case ScalingMode.Uniform:
					scale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
					break;
				case ScalingMode.Width:
					scale = new Vector3(scaleFactor, 0, 0);
					break;
				case ScalingMode.Height:
					scale = new Vector3(0, scaleFactor, 0);
					break;
				case ScalingMode.Depth:
					scale = new Vector3(0, 0, scaleFactor);
					break;
			}

			var newLocalScale = SelectedObject.transform.localScale + scale;
			newLocalScale.x = Mathf.Max(newLocalScale.x, 0.0f);
			newLocalScale.y = Mathf.Max(newLocalScale.y, 0.0f);
			newLocalScale.z = Mathf.Max(newLocalScale.z, 0.0f);

			SelectedObject.transform.localScale = newLocalScale;
		}

		private float GetScaleSnappingIncrement()
		{
			float increment = 0.0f;

			switch (Settings.Instance.ScaleSnappingMode)
			{
				case ScaleSnappingMode.Off:
					increment = 0.0f;
					break;
				case ScaleSnappingMode.Quarter:
					increment = 0.25f;
					break;
				case ScaleSnappingMode.Half:
					increment = 0.5f;
					break;
				case ScaleSnappingMode.Full:
					increment = 1.0f;
					break;
			}

			return increment;
		}
	}
}
