using System;
using UnityEngine;
using XLObjectDropper.UI.Controls;
using XLObjectDropper.UI.Controls.ControllerButtons;
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

		[HideInInspector] public RotationSnappingMode RotationSnappingMode;
		[HideInInspector] public ScalingMode ScalingMode;
		[HideInInspector] public ScaleSnappingMode ScaleSnappingMode;

		private void Awake()
		{
			DPad.gameObject.SetActive(false);

			RightStick.EnableStickButton(true);
			LeftStick.EnableStickButton(true);
		}

		private void Start()
		{
			SetScaleModeText();
			SetRotationSnappingText();
			SetScaleSnappingText();
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
			RotationSnappingMode++;

			if ((int)RotationSnappingMode > Enum.GetValues(typeof(RotationSnappingMode)).Length - 1)
				RotationSnappingMode = RotationSnappingMode.Off;

			SetRotationSnappingText();
		}

		private void SetRotationSnappingText()
		{
			string rotationSnapping = "Off";
			switch (RotationSnappingMode)
			{
				case RotationSnappingMode.Off:
					rotationSnapping = "Off";
					break;
				case RotationSnappingMode.Degrees15:
					rotationSnapping = "15°";
					break;
				case RotationSnappingMode.Degrees45:
					rotationSnapping = "45°";
					break;
				case RotationSnappingMode.Degrees90:
					rotationSnapping = "90°";
					break;
			}

			DPad.gameObject.SetActive(RotationSnappingMode > 0);

			AXBYButtons.XButton.ButtonLabel.SetText($"Rotation Snapping: {Utilities.Color.ColorTag}{rotationSnapping}");
		}

		private void UpdateScaleMode()
		{
			ScalingMode++;

			if ((int)ScalingMode > Enum.GetValues(typeof(ScalingMode)).Length - 1)
				ScalingMode = ScalingMode.Uniform;

			SetScaleModeText();
		}

		private void SetScaleModeText()
		{
			string scalingMode = "Uniform";
			switch (ScalingMode)
			{
				case ScalingMode.Uniform:
					scalingMode = "Uniform";
					break;
				case ScalingMode.Width:
					scalingMode = "Width";
					break;
				case ScalingMode.Height:
					scalingMode = "Height";
					break;
				case ScalingMode.Depth:
					scalingMode = "Depth";
					break;
			}

			AXBYButtons.YButton.ButtonLabel.SetText($"Scale: {Utilities.Color.ColorTag}{scalingMode}");
		}

		private void UpdateScaleSnappingMode()
		{
			ScaleSnappingMode++;

			if ((int)ScaleSnappingMode > Enum.GetValues(typeof(ScaleSnappingMode)).Length - 1)
				ScaleSnappingMode = ScaleSnappingMode.Off;

			SetScaleSnappingText();
		}

		private void SetScaleSnappingText()
		{
			string scalingMode = "Uniform";
			switch (ScaleSnappingMode)
			{
				case ScaleSnappingMode.Off:
					scalingMode = "Off";
					break;
				case ScaleSnappingMode.Quarter:
					scalingMode = "¼";
					break;
				case ScaleSnappingMode.Half:
					scalingMode = "½";
					break;
				case ScaleSnappingMode.Full:
				default:
					scalingMode = "1";
					break;
			}

			AXBYButtons.AButton.ButtonLabel.SetText($"Scale Snapping: {Utilities.Color.ColorTag}{scalingMode}");
		}
	}
}
