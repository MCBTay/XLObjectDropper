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

		[Space(10)]
		public GameObject SnappingModeUI;
		public GameObject RotateAndScaleModeUI;
		[Space(10)]
		public GameObject RightTrigger;
		public GameObject RightTrigger_Pressed;
		public GameObject RightBumper;
		public TMP_Text RightBumperLabel;
		[HideInInspector] private bool RightBumperEnabled;
        public GameObject LeftTrigger;
        public GameObject LeftTrigger_Pressed;
		public GameObject LeftBumper;
        public TMP_Text LeftBumperLabel;
        [HideInInspector] private bool LeftBumperEnabled;
        [Space(10)]
        public GameObject DirectionalPad;
		public GameObject AXYBButtons;

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
			RightTrigger.SetActive(false);
			LeftTrigger.SetActive(false);
        }

		private void OnDisable()
		{
			MainScreen_UI.SetActive(false);

			// Object Placement
			SnappingModeUI.SetActive(false);
			RotateAndScaleModeUI.SetActive(false);
			RightTrigger.SetActive(false);
			LeftTrigger.SetActive(false);
        }

		private void Start()
		{
            MainScreen_UI.SetActive(true);

			// Object Placement
			SnappingModeUI.SetActive(false);
			RotateAndScaleModeUI.SetActive(false);
			RightTrigger.SetActive(false);
			LeftTrigger.SetActive(false);
		}

		private void Update()
		{
			var player = UIManager.Instance.Player;

			#region Right bumper
			if (RightBumperEnabled)
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
            if (LeftBumperEnabled)
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

            #region Right Trigger
            if (player.GetButton("RT"))
            {
                RightTrigger.SetActive(false);
				RightTrigger_Pressed.SetActive(true);
            }
            else
            {
				RightTrigger.SetActive(true);
				RightTrigger_Pressed.SetActive(false);
			}
            #endregion

            #region Left Trigger
            if (player.GetButton("LT"))
			{
				LeftTrigger.SetActive(false);
				LeftTrigger_Pressed.SetActive(true);
            }
            else
            {
	            LeftTrigger.SetActive(true);
	            LeftTrigger_Pressed.SetActive(false);
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
                directionalPad.RightLabel.SetText($"Lock Cam: <color=#3286EC>{(LockCam ? "On" : "Off")}");
            }

            EnableRightBumper(HasSelectedObject);
            EnableLeftBumper(HasSelectedObject);
		}

		public void EnableRightBumper(bool buttonEnabled)
		{
			RightBumperEnabled = buttonEnabled;
			RightBumperLabel.alpha = GetAlpha(buttonEnabled);
            RightBumper.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, GetAlpha(buttonEnabled));
		}

		public void EnableLeftBumper(bool buttonEnabled)
		{
			LeftBumperEnabled = buttonEnabled;
			LeftBumperLabel.alpha = GetAlpha(buttonEnabled);
			LeftBumper.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, GetAlpha(buttonEnabled));
        }

        private float GetAlpha(bool buttonEnabled) { return buttonEnabled ? 1.0f : 0.3f; }
    }
}
