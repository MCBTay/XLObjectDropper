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

		[HideInInspector] public MovementSnappingMode MovementSnappingMode;

		private void Start()
		{
			SetMovementSnappingText();
		}

		private void Update()
		{
			var player = UIManager.Instance.Player;

			if (player.GetButtonDown("X")) UpdateMovementSnappingMode();
		}

		private void UpdateMovementSnappingMode()
		{
			MovementSnappingMode++;

			if ((int)MovementSnappingMode > Enum.GetValues(typeof(MovementSnappingMode)).Length - 1)
				MovementSnappingMode = MovementSnappingMode.Off;

			SetMovementSnappingText();
		}

		private void SetMovementSnappingText()
		{
			string placementSnapping = "Off";
			switch (MovementSnappingMode)
			{
				case MovementSnappingMode.Off:
					placementSnapping = "Off";
					break;
				case MovementSnappingMode.Quarter:
					placementSnapping = "¼m";
					break;
				case MovementSnappingMode.Half:
					placementSnapping = "½m";
					break;
				case MovementSnappingMode.Full:
					placementSnapping = "1m";
					break;
				case MovementSnappingMode.Double:
					placementSnapping = "2m";
					break;
			}

			AXBYButtons.XButton.ButtonLabel.SetText($"Movement Snapping: {Utilities.Color.ColorTag}{placementSnapping}");
		}
	}
}
