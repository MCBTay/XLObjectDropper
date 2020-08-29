using System;
using UnityEngine;

namespace XLObjectDropper.UI
{
	public class RotationAndScaleUI : MonoBehaviour
	{
		public GameObject AXBYButtons;

		[HideInInspector] private int CurrentRotationSnappingMode;

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
			AXBYButtons.GetComponent<AXYBController>().SetXButtonLabelText("ROTATION SNAPPING: <color=#3286EC>OFF");
		}

		private void OnDisable()
		{
			SetDefaultState(false);
		}

		private void Update()
		{
			if (UIManager.Instance.Player.GetButtonUp("X"))
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
		}
	}
}
