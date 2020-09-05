using System;
using UnityEngine;

namespace XLObjectDropper.UI
{
	public class RotationAndScaleUI : MonoBehaviour
	{
		public GameObject AXBYButtons;

		[HideInInspector] private int CurrentRotationSnappingMode;
		[HideInInspector] private int CurrentScaleMode;

		private void OnEnable()
		{
			SetDefaultState(true);
		}

		private void Start()
		{
			SetDefaultState(true);
		}

		private void SetDefaultState(bool enabled)
		{
			var controller = AXBYButtons.GetComponent<AXYBController>();
			if (controller != null)
			{
				controller.SetXButtonLabelText("ROTATION SNAPPING: <color=#3286EC>OFF");
				controller.SetYButtonLabelText("SCALE: <color=#3286EC>UNIFORM");
			}
			
		}

		private void OnDisable()
		{
			SetDefaultState(false);
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
