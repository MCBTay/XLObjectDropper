using System;
using UnityEngine;
using XLObjectDropper.UI.Controls;
using XLObjectDropper.UI.Utilities;

namespace XLObjectDropper.UI
{
	public class RotationAndScaleUI : MonoBehaviour
	{
		public GameObject AXBYButtons;
		public GameObject DPad;

		[HideInInspector] private static int CurrentRotationSnappingMode;
		[HideInInspector] private static int CurrentScaleMode;

		private void Awake()
		{
			CurrentScaleMode = (int) ScalingMode.Uniform;
			CurrentRotationSnappingMode = (int) RotationSnappingMode.Off;

			DPad.SetActive(false);
		}

		private void Update()
		{
			if (UIManager.Instance.Player.GetButtonDown("X"))
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

				if (CurrentRotationSnappingMode > 0)
				{
					DPad.SetActive(true);
				}
				else
				{
					DPad.SetActive(false);
				}

				AXBYButtons.GetComponent<AXYBController>().XButton.SetLabelText($"Rotation Snapping: {Utilities.Color.ColorTag}{rotationSnapping}");
			}

			if (UIManager.Instance.Player.GetButtonDown("Y"))
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

				AXBYButtons.GetComponent<AXYBController>().YButton.SetLabelText($"Scale: {Utilities.Color.ColorTag}{scalingMode}");
			}
		}
	}
}
