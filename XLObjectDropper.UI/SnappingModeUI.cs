using System;
using UnityEngine;
using XLObjectDropper.UI.Controls;
using XLObjectDropper.UI.Utilities;

namespace XLObjectDropper.UI
{
	public class SnappingModeUI : MonoBehaviour
	{
		public AXYBController AXBYButtons;
		public DirectionalPadController DPad;

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

			string placementSnapping = "Off";
			switch (CurrentMovementSnappingMode)
			{
				case (int)MovementSnappingMode.Off:
					placementSnapping = "Off";
					break;
				case (int)MovementSnappingMode.Quarter:
					placementSnapping = "¼m";
					break;
				case (int)MovementSnappingMode.Half:
					placementSnapping = "½m";
					break;
				case (int)MovementSnappingMode.Full:
					placementSnapping = "1m";
					break;
				case (int)MovementSnappingMode.Double:
					placementSnapping = "2m";
					break;
			}

			AXBYButtons.XButton.ButtonLabel.SetText($"Movement Snapping: {Utilities.Color.ColorTag}{placementSnapping}");
		}
	}
}
