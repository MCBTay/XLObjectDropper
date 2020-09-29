using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XLObjectDropper.UI.Controls;

namespace XLObjectDropper.UI
{
	public class ObjectPlacementUI : MonoBehaviour
	{
		[Header("Object Placement Elements")]
		public GameObject MainScreen_UI;
		public GameObject SnappingModeUI;
		public GameObject RotateAndScaleModeUI;
		[Header("Bumpers and Triggers")]
		public TriggerController Triggers;
		public BumperController Bumpers;
        [Space(10)]
        public DirectionalPadController DirectionalPad;
		public AXYBController AXYBButtons;
		[Header("Sticks")]
		public StickController LeftStick;
		public StickController RightStick;
		[Space(10)]
		public BottomRowController BottomRow;
		public GameObject Cursor;

		[HideInInspector] public bool HasHighlightedObject;
		[HideInInspector] public bool HasSelectedObject;
		[HideInInspector] private bool LockCam;
		[HideInInspector] public bool GroundTracking;

		private void OnEnable()
		{
			MainScreen_UI.SetActive(true);

			// Object Placement
			SnappingModeUI.SetActive(false);
			RotateAndScaleModeUI.SetActive(false);
		}

		private void OnDisable()
		{
			MainScreen_UI.SetActive(false);

			// Object Placement
			SnappingModeUI.SetActive(false);
			RotateAndScaleModeUI.SetActive(false);
		}

		private void Start()
		{
            MainScreen_UI.SetActive(true);

			// Object Placement
			SnappingModeUI.SetActive(false);
			RotateAndScaleModeUI.SetActive(false);
		}

		private void Update()
		{
			var player = UIManager.Instance.Player;

			#region Right bumper
			if (HasSelectedObject)
			{
				if (player.GetButtonDown("RB"))
				{
					MainScreen_UI.SetActive(false);
					SnappingModeUI.SetActive(true);
				}
				if (player.GetButton("RB")) return;
				if (player.GetButtonUp("RB"))
				{
					MainScreen_UI.SetActive(true);
					SnappingModeUI.SetActive(false);
				}
			}
			else
			{
				MainScreen_UI.SetActive(true);
				SnappingModeUI.SetActive(false);
			}
			#endregion

            #region Left Bumper
            if (HasSelectedObject)
            {
	            if (player.GetButtonDown("LB"))
	            {
		            MainScreen_UI.SetActive(false);
		            RotateAndScaleModeUI.SetActive(true);
	            }
	            if (player.GetButton("LB")) return;
	            if (player.GetButtonUp("LB"))
	            {
		            MainScreen_UI.SetActive(true);
		            RotateAndScaleModeUI.SetActive(false);
	            }
            }
            else
            {
				MainScreen_UI.SetActive(true);
				RotateAndScaleModeUI.SetActive(false);
			}
            #endregion

            if (player.GetButtonDown("DPadX")) LockCam = !LockCam;
            if (player.GetNegativeButtonDown("DPadX")) GroundTracking = !GroundTracking;

            AXYBButtons.XButton.UpdateButton("Edit", HasSelectedObject || HasHighlightedObject);
			AXYBButtons.YButton.UpdateButton("Delete", HasSelectedObject || HasHighlightedObject);
			AXYBButtons.AButton.UpdateButton($"{(HasSelectedObject ? "Place" : "Select")}/Duplicate", HasSelectedObject || HasHighlightedObject);
			AXYBButtons.BButton.UpdateButton(HasSelectedObject ? "Cancel" : "Exit");

			DirectionalPad.RightLabel.SetText($"Lock Cam: {Utilities.Color.ColorTag}{(LockCam ? "On" : "Off")}");
			DirectionalPad.LeftLabel.SetText($"Ground Tracking: {Utilities.Color.ColorTag}{(GroundTracking ? "On" : "Off")}");
			
			Bumpers.RightBumper.EnableButton(HasSelectedObject);
			Bumpers.LeftBumper.EnableButton(HasSelectedObject);

			RightStick.EnableStickButton(true);
			LeftStick.EnableStickButton(HasSelectedObject);

			//BottomRow.EnableSelectButton = BottomRow.EnableStartButton = true;
		}
	}
}
