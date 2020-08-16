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

        public static UI_Manager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            RB_UI.SetActive(false);
            LB_UI.SetActive(false);
            ObjectSelection_Master.SetActive(false);
            OptionsMenu_Master.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKey(KeyCode.Joystick1Button5))
            {
                MainScreen_UI.SetActive(false);
                RB_UI.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.Joystick1Button5))
            {
                MainScreen_UI.SetActive(true);
                RB_UI.SetActive(false);
            }
            if (Input.GetKey(KeyCode.Joystick1Button4))
            {
                MainScreen_UI.SetActive(false);
                LB_UI.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.Joystick1Button4))
            {
                MainScreen_UI.SetActive(true);
                RT_UI.SetActive(false);
            }
            if (Input.GetKey(KeyCode.Joystick1Button4))
            {
                MainScreen_UI.SetActive(false);
                RT_UI.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.Joystick1Button4))
            {
                MainScreen_UI.SetActive(true);
                RT_UI.SetActive(false);
            }
        }
    }
}