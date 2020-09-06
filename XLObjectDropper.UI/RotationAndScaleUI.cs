using System;
using UnityEngine;

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

				string rotationSnapping = "OFF";
				switch (CurrentRotationSnappingMode)
				{
					case (int)RotationSnappingMode.Off:
						rotationSnapping = "OFF";
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

				AXBYButtons.GetComponent<AXYBController>().SetXButtonLabelText($"ROTATION SNAPPING: <color=#3286EC>{rotationSnapping}");
			}

			if (UIManager.Instance.Player.GetButtonDown("Y"))
			{
				CurrentScaleMode++;

				if (CurrentScaleMode > Enum.GetValues(typeof(ScalingMode)).Length - 1)
					CurrentScaleMode = 0;

				string scalingMode = "UNIFORM";
				switch (CurrentScaleMode)
				{
					case (int)ScalingMode.Uniform:
						scalingMode = "UNIFORM";
						break;
					case (int)ScalingMode.Width:
						scalingMode = "WIDTH";
						break;
					case (int)ScalingMode.Height:
						scalingMode = "HEIGHT";
						break;
					case (int)ScalingMode.Depth:
						scalingMode = "DEPTH";
						break;
				}

				AXBYButtons.GetComponent<AXYBController>().SetYButtonLabelText($"SCALE: <color=#3286EC>{scalingMode}");
			}
		}
	}
}
