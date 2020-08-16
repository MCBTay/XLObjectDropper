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
            Debug.Log("[XLObjectDropper.UI] UI_Manager version 0.1.6");
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
            // Right Bumper
            if (Input.GetKeyDown(KeyCode.Joystick1Button5))
            {
                MainScreen_UI.SetActive(false);
                RB_UI.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.Joystick1Button5))
            {
                MainScreen_UI.SetActive(true);
                RB_UI.SetActive(false);
            }
            // Left Bumper
            if (Input.GetKeyDown(KeyCode.Joystick1Button4))
            {
                MainScreen_UI.SetActive(false);
                LB_UI.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.Joystick1Button4))
            {
                MainScreen_UI.SetActive(true);
                LB_UI.SetActive(false);
            }
            // Right Trigger
            /// Rewired
            var axisTest_RT = PlayerController.Instance.inputController.player.GetAxis("RT");
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
            // Left Trigger
            /// Rewired
            var axisTest_LT = PlayerController.Instance.inputController.player.GetAxis("LT");
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
            //DPad
            ///Rewired
            var axisTest_DPadX = PlayerController.Instance.inputController.player.GetAxis("DPadX");
            if (axisTest_DPadX < 0)
            {
                Dpad_Left.SetActive(true);
            }
            if (axisTest_DPadX > 0)
            {
                Dpad_Up.SetActive(true);
            }
            var axisTest_DPadY = PlayerController.Instance.inputController.player.GetAxis("DPadY");
            if (axisTest_DPadY < 0)
            {
                Dpad_Down.SetActive(true);
            }
            if (axisTest_DPadY > 0)
            {
                Dpad_Up.SetActive(true);
            }
            else
            {
                Dpad_Down.SetActive(false);
                Dpad_Up.SetActive(false);
                Dpad_Left.SetActive(false);
                Dpad_Right.SetActive(false);
            }
        }
    }
}