using System;
using UnityEngine;
using XLObjectDropper.UI.Controls;
using XLObjectDropper.UI.Utilities;

namespace XLObjectDropper.UI
{
	public class SnappingModeUI : MonoBehaviour
	{
		public GameObject AXBYButtons;
		public GameObject DPad;

		[HideInInspector] private static int CurrentMovementSnappingMode;

		private void Awake()
		{
			CurrentMovementSnappingMode = (int) MovementSnappingMode.Off;
		}

		private void Update()
		{
			var player = UIManager.Instance.Player;

			if (player.GetButtonDown("X")) UpdateMovementSnappingMode();
		}

		private void UpdateMovementSnappingMode()
		{
			CurrentMovementSnappingMode++;

			if (CurrentMovementSnappingMode > Enum.GetValues(typeof(MovementSnappingMode)).Length - 1)
				CurrentMovementSnappingMode = 0;

			string placementSnapping = "OFF";
			switch (CurrentMovementSnappingMode)
			{
				case (int)MovementSnappingMode.Off:
					placementSnapping = "OFF";
					break;
				case (int)MovementSnappingMode.Quarter:
					placementSnapping = "1/4m";
					break;
				case (int)MovementSnappingMode.Half:
					placementSnapping = "1/2m";
					break;
				case (int)MovementSnappingMode.Full:
					placementSnapping = "1m";
					break;
				case (int)MovementSnappingMode.Double:
					placementSnapping = "2m";
					break;
			}

			AXBYButtons.GetComponent<AXYBController>().SetXButtonLabelText($"MOVEMENT SNAPPING: <color=#3286EC>{placementSnapping}");
		}
	}
}
