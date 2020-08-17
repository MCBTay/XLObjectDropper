using UnityEngine;
using UnityEngine.UI;

namespace XLObjectDropper.UI
{
    public class UI_Manager : MonoBehaviour
    {
        [Header("Master Elements")]
        // Master Elements
        public GameObject ObjectPlacement_Master;
        public GameObject ObjectSelection_Master;
        public GameObject OptionsMenu_Master;
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
        public GameObject Dpad_Up;
        public GameObject Dpad_Down;
        public GameObject Dpad_Left;
        public GameObject Dpad_Right;

        public static UI_Manager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Debug.Log("[XLObjectDropper.UI] UI_Manager.dll Initialized");
            Debug.Log("[XLObjectDropper.UI] UI_Manager version 0.1.7");
            // Hide UI at start
            // Master UIs
            ObjectSelection_Master.SetActive(false);
            OptionsMenu_Master.SetActive(false);
            // Object Placement
            RB_UI.SetActive(false);
            LB_UI.SetActive(false);
            RT_UI.SetActive(false);
            LT_UI.SetActive(false);
            Dpad_Down.SetActive(false);
            Dpad_Up.SetActive(false);
            Dpad_Left.SetActive(false);
            Dpad_Right.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
	        var player = PlayerController.Instance.inputController.player;


            #region ObjectSelectionMenu
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
            var axisTest_RT = player.GetAxis("RT");
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
            var axisTest_LT = player.GetAxis("LT");
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

            #region Directional Pad
            #region Directional Pad X
            var axisTest_DPadX = player.GetAxis("DPadX");
            if (axisTest_DPadX < 0)
            {
                Dpad_Left.SetActive(true);
            }
            else
            {
                Dpad_Left.SetActive(false);
            }
            if (axisTest_DPadX > 0)
            {
                Dpad_Right.SetActive(true);
            }
            else
            {
                Dpad_Right.SetActive(false);
            }
            #endregion

            #region Directional Pad Y
            var axisTest_DPadY = player.GetAxis("DPadY");
            if (axisTest_DPadY < 0)
            {
                Dpad_Down.SetActive(true);
            }
            else
            {
                Dpad_Down.SetActive(false);
            }
            if (axisTest_DPadY > 0)
            {
                Dpad_Up.SetActive(true);
            }
            else
            {
                Dpad_Up.SetActive(false);
            }
            #endregion
            #endregion
            #endregion

            #region OptionsMenu
            if (player.GetButtonDown("Select"))
            {
                if (OptionsMenu_Master.activeInHierarchy == false)
                {
                    MainScreen_UI.SetActive(false);
                    OptionsMenu_Master.SetActive(true);
                }
                else
                {
                    OptionsMenu_Master.SetActive(false);
                    MainScreen_UI.SetActive(true);
                }
            }
            #endregion
        }
    }
}