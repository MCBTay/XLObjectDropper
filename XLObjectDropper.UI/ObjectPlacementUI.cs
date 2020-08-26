using TMPro;
using UnityEngine;

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
		
        [Space(10)]
        public TMP_Text RotationSnappingStateText;

		private void Start()
		{
			// Object Placement
			RB_UI.SetActive(false);
			LB_UI.SetActive(false);
			RT_UI.SetActive(false);
			LT_UI.SetActive(false);
		}

		private void Update()
		{
			//var Player = PlayerController.Instance.inputController.Player;

            #region Right bumper
            if (UIManager.Instance.Player.GetButtonDown("RB"))
            {
                MainScreen_UI.SetActive(false);
                RB_UI.SetActive(true);
            }
            if (UIManager.Instance.Player.GetButtonUp("RB"))
            {
                MainScreen_UI.SetActive(true);
                RB_UI.SetActive(false);
            }
            #endregion

            #region Left Bumper
            if (UIManager.Instance.Player.GetButtonDown("LB"))
            {
                MainScreen_UI.SetActive(false);
                LB_UI.SetActive(true);
            }
            if (UIManager.Instance.Player.GetButtonUp("LB"))
            {
                MainScreen_UI.SetActive(true);
                LB_UI.SetActive(false);
            }
            #endregion

            #region Right Trigger
            var axisTest_RT = UIManager.Instance.Player.GetAxis("RT");
            if (axisTest_RT > .1)
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
            var axisTest_LT = UIManager.Instance.Player.GetAxis("LT");
            if (axisTest_LT > .1)
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
		}
    }
}
