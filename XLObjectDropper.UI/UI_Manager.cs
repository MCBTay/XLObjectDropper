using UnityEngine;
using UnityEngine.UI;

namespace XLObjectDropper.UI
{
    public class UI_Manager : MonoBehaviour
    {
        public GameObject MainScreenUI;
        public GameObject RB_UI;
        public Sprite RB_Active;
        private Sprite RB_Inactive;

        public static UI_Manager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKey(KeyCode.Joystick1Button5))
            {
                var trigger = RB_UI.GetComponent<Image>();
                RB_Inactive = trigger.sprite;
                trigger.sprite = RB_Active;

                Debug.Log("Right Bumper Held");
            }
            if (Input.GetKeyUp(KeyCode.Joystick1Button5))
            {
                RB_UI.GetComponent<Image>().sprite = RB_Inactive;
                Debug.Log("Right Bumper Released");
            }
        }
    }
}