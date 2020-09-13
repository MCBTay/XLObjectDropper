using System;
using TMPro;
using UnityEngine;
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
		public GameObject RB_UI;
		public GameObject LB_UI;
		[Space(10)]
		public GameObject RT_UI;
		public GameObject LT_UI;

		public GameObject DirectionalPad;

		public GameObject Cursor;

		[HideInInspector] private static int CurrentPlacementSnappingMode;

		private void OnEnable()
		{
			MainScreen_UI.SetActive(true);

			// Object Placement
			RB_UI.SetActive(false);
			LB_UI.SetActive(false);
			RT_UI.SetActive(false);
			LT_UI.SetActive(false);
        }

		private void OnDisable()
		{
			MainScreen_UI.SetActive(false);

			// Object Placement
			RB_UI.SetActive(false);
			LB_UI.SetActive(false);
			RT_UI.SetActive(false);
			LT_UI.SetActive(false);
        }

		private void Start()
		{
            MainScreen_UI.SetActive(true);

			// Object Placement
			RB_UI.SetActive(false);
			LB_UI.SetActive(false);
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
                RB_UI.SetActive(true);
            }
            if (player.GetButtonUp("RB"))
            {
                MainScreen_UI.SetActive(true);
                RB_UI.SetActive(false);
            }
            #endregion

            #region Left Bumper
            if (player.GetButtonDown("LB"))
            {
                MainScreen_UI.SetActive(false);
                LB_UI.SetActive(true);
            }
            if (player.GetButtonUp("LB"))
            {
                MainScreen_UI.SetActive(true);
                LB_UI.SetActive(false);
            }
            #endregion

            #region Right Trigger
            if (player.GetButton("RT"))
            {
                if (LB_UI.activeInHierarchy == false)
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
                if (LB_UI.activeInHierarchy == false)
                {
                    LT_UI.SetActive(true);
                }
            }
            else
            {
                LT_UI.SetActive(false);
            }
            #endregion

            if (player.GetNegativeButtonDown("DPadX"))
            {
	            CurrentPlacementSnappingMode++;

	            if (CurrentPlacementSnappingMode > Enum.GetValues(typeof(PlacementSnappingMode)).Length - 1)
		            CurrentPlacementSnappingMode = 0;

	            string placementSnapping = "OFF";
	            switch (CurrentPlacementSnappingMode)
	            {
		            case (int)PlacementSnappingMode.Off:
			            placementSnapping = "OFF";
			            break;
		            case (int)PlacementSnappingMode.Quarter:
			            placementSnapping = "1/4m";
			            break;
		            case (int)PlacementSnappingMode.Half:
			            placementSnapping = "1/2m";
			            break;
		            case (int)PlacementSnappingMode.Full:
			            placementSnapping = "1m";
			            break;
					case (int)PlacementSnappingMode.Double:
						placementSnapping = "2m";
						break;
	            }

				DirectionalPad.GetComponent<DirectionalPadController>().LeftLabel.SetText($"SNAP PLACEMENT: <color=#3286EC>{placementSnapping}");
            }
		}
    }
}
