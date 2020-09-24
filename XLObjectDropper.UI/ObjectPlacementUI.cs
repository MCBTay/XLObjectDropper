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
		public GameObject Triggers;
		public GameObject Bumpers;
        [Space(10)]
        public GameObject DirectionalPad;
		public GameObject AXYBButtons;
		[Header("Sticks")]
		public GameObject LeftStick;
		public GameObject RightStick;
		[Space(10)]
		public GameObject Cursor;

		[HideInInspector] private static int CurrentPlacementSnappingMode;
		[HideInInspector] public bool HasHighlightedObject;
		[HideInInspector] public bool HasSelectedObject;
		[HideInInspector] private bool LockCam;

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

            var buttonController = AXYBButtons.GetComponentInChildren<AXYBController>();
            if (buttonController != null)
            {
				buttonController.XButton.UpdateButton("Duplicate", HasSelectedObject || HasHighlightedObject);
				buttonController.YButton.UpdateButton("Delete", HasSelectedObject || HasHighlightedObject);
	            buttonController.AButton.UpdateButton(HasSelectedObject ? "Place" : "Select", HasSelectedObject || HasHighlightedObject);
	            buttonController.BButton.UpdateButton(HasSelectedObject ? "Cancel" : "Exit");
            }

            var directionalPad = DirectionalPad.GetComponentInChildren<DirectionalPadController>();
            if (directionalPad != null)
            {
                directionalPad.RightLabel.SetText($"Lock Cam: {Utilities.Color.ColorTag}{(LockCam ? "On" : "Off")}");
            }

			Bumpers.GetComponent<BumperController>().RightBumper.EnableButton(HasSelectedObject);
			Bumpers.GetComponent<BumperController>().LeftBumper.EnableButton(HasSelectedObject);

			RightStick.GetComponent<StickController>().EnableStickButton(true);
			LeftStick.GetComponent<StickController>().EnableStickButton(HasSelectedObject);
		}
	}
}
