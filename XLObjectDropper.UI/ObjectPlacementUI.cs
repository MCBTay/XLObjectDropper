using System;
using UnityEngine;
using UnityEngine.UI;
using XLObjectDropper.UI.Controls;
using XLObjectDropper.UI.Utilities;

namespace XLObjectDropper.UI
{
	public class ObjectPlacementUI : MonoBehaviour
	{
		[Header("Object Placement Elements")]
		// Object Placement
		public GameObject MainScreen_UI;

		[Space(10)]
		public GameObject SnappingModeUI;
		public GameObject RotateAndScaleModeUI;
		[Space(10)]
		public GameObject RT_UI;
		public GameObject LT_UI;

		public GameObject DirectionalPad;
		public GameObject AXYBButtons;

		public GameObject Cursor;

		[HideInInspector] private static int CurrentPlacementSnappingMode;
		[HideInInspector] public bool HasHighlightedObject;
		[HideInInspector] public bool HasSelectedObject;

		private void OnEnable()
		{
			MainScreen_UI.SetActive(true);

			// Object Placement
			SnappingModeUI.SetActive(false);
			RotateAndScaleModeUI.SetActive(false);
			RT_UI.SetActive(false);
			LT_UI.SetActive(false);
        }

		private void OnDisable()
		{
			MainScreen_UI.SetActive(false);

			// Object Placement
			SnappingModeUI.SetActive(false);
			RotateAndScaleModeUI.SetActive(false);
			RT_UI.SetActive(false);
			LT_UI.SetActive(false);
        }

		private void Start()
		{
            MainScreen_UI.SetActive(true);

			// Object Placement
			SnappingModeUI.SetActive(false);
			RotateAndScaleModeUI.SetActive(false);
			RT_UI.SetActive(false);
			LT_UI.SetActive(false);
		}

		private void Update()
		{
			var player = UIManager.Instance.Player;

			#region Right bumper
            if (player.GetButtonDown("RB"))
            {
                MainScreen_UI.SetActive(false);
                SnappingModeUI.SetActive(true);
            }
            if (player.GetButtonUp("RB"))
            {
                MainScreen_UI.SetActive(true);
                SnappingModeUI.SetActive(false);
            }
            #endregion

            #region Left Bumper
            if (player.GetButtonDown("LB"))
            {
                MainScreen_UI.SetActive(false);
                RotateAndScaleModeUI.SetActive(true);
            }
            if (player.GetButtonUp("LB"))
            {
                MainScreen_UI.SetActive(true);
                RotateAndScaleModeUI.SetActive(false);
            }
            #endregion

            #region Right Trigger
            if (player.GetButton("RT"))
            {
                if (RotateAndScaleModeUI.activeInHierarchy == false)
                {
                    RT_UI.SetActive(true);
                }
            }
            else
            {
                RT_UI.SetActive(false);
            }
            #endregion

            #region Left Trigger
            if (player.GetButton("LT"))
            {
                if (RotateAndScaleModeUI.activeInHierarchy == false)
                {
                    LT_UI.SetActive(true);
                }
            }
            else
            {
                LT_UI.SetActive(false);
            }
            #endregion

            var buttonController = AXYBButtons.GetComponentInChildren<AXYBController>();
            if (buttonController != null)
            {
				buttonController.XButton.UpdateButton("Duplicate", HasSelectedObject || HasHighlightedObject);
				buttonController.YButton.UpdateButton("Delete", HasSelectedObject || HasHighlightedObject);
	            buttonController.AButton.UpdateButton(HasSelectedObject ? "Place" : "Select", HasSelectedObject || HasHighlightedObject);
	            buttonController.BButton.UpdateButton(HasSelectedObject ? "Cancel" : "Exit");
            }
		}
    }
}
