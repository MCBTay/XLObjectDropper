using System;
using UnityEngine;
using XLObjectDropper.UI.Controls;
using XLObjectDropper.UI.Utilities;

namespace XLObjectDropper.UI
{
	public class RotationAndScaleUI : MonoBehaviour
	{
		public AXYBController AXBYButtons;
		public DirectionalPadController DPad;
		[Header("Sticks")]
		public StickController LeftStick;
		public StickController RightStick;

		[HideInInspector] private static int CurrentRotationSnappingMode;
		[HideInInspector] private static int CurrentScaleMode;
		[HideInInspector] private static int CurrentScaleSnappingMode;

		private void Awake()
		{
			CurrentScaleMode = (int) ScalingMode.Uniform;
			CurrentRotationSnappingMode = (int) RotationSnappingMode.Off;

			DPad.gameObject.SetActive(false);

			RightStick.EnableStickButton(true);
			LeftStick.EnableStickButton(true);
		}

		private void Update()
		{
			var player = UIManager.Instance.Player;

			if (player.GetButtonDown("A")) UpdateScaleSnappingMode();
			if (player.GetButtonDown("X")) UpdateRotationSnappingMode();
			if (player.GetButtonDown("Y")) UpdateScaleMode();
		}

		private void UpdateRotationSnappingMode()
		{
			CurrentRotationSnappingMode++;

			if (CurrentRotationSnappingMode > Enum.GetValues(typeof(RotationSnappingMode)).Length - 1)
				CurrentRotationSnappingMode = 0;

			string rotationSnapping = "Off";
			switch (CurrentRotationSnappingMode)
			{
				case (int)RotationSnappingMode.Off:
					rotationSnapping = "Off";
					break;
				case (int)RotationSnappingMode.Degrees15:
					rotationSnapping = "15°";
					break;
				case (int)RotationSnappingMode.Degrees45:
					rotationSnapping = "45°";
					break;
				case (int)RotationSnappingMode.Degrees90:
					rotationSnapping = "90°";
					break;
			}

			DPad.gameObject.SetActive(CurrentRotationSnappingMode > 0);

			AXBYButtons.XButton.ButtonLabel.SetText($"Rotation Snapping: {Utilities.Color.ColorTag}{rotationSnapping}");
		}

		private void UpdateScaleMode()
		{
			CurrentScaleMode++;

			if (CurrentScaleMode > Enum.GetValues(typeof(ScalingMode)).Length - 1)
				CurrentScaleMode = 0;

			string scalingMode = "Uniform";
			switch (CurrentScaleMode)
			{
				case (int)ScalingMode.Uniform:
					scalingMode = "Uniform";
					break;
				case (int)ScalingMode.Width:
					scalingMode = "Width";
					break;
				case (int)ScalingMode.Height:
					scalingMode = "Height";
					break;
				case (int)ScalingMode.Depth:
					scalingMode = "Depth";
					break;
			}

			AXBYButtons.YButton.ButtonLabel.SetText($"Scale: {Utilities.Color.ColorTag}{scalingMode}");
		}

		private void UpdateScaleSnappingMode()
		{
			CurrentScaleSnappingMode++;

			if (CurrentScaleSnappingMode > Enum.GetValues(typeof(ScaleSnappingMode)).Length - 1)
				CurrentScaleSnappingMode = 0;

			string scalingMode = "Uniform";
			switch (CurrentScaleSnappingMode)
			{
				case (int)ScaleSnappingMode.Off:
					scalingMode = "Off";
					break;
				case (int)ScaleSnappingMode.Quarter:
					scalingMode = "¼";
					break;
				case (int)ScaleSnappingMode.Half:
					scalingMode = "½";
					break;
				case (int)ScaleSnappingMode.Full:
				default:
					scalingMode = "1";
					break;
			}

			AXBYButtons.AButton.ButtonLabel.SetText($"Scale Snapping: {Utilities.Color.ColorTag}{scalingMode}");
		}
	}
}
