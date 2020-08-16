using UnityEngine;

namespace XLObjectDropper.UI
{
    public class UI_Manager : MonoBehaviour
    {
        public GameObject MainScreenUI;
        public GameObject RB_UI;
        public Sprite RB_Active;

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
                MainScreenUI.SetActive(false);
                RB_UI.GetComponent<Image>().sprite = RB_Active;

                Debug.Log("Right Bumper Held");
            }
            if (Input.GetKeyUp(KeyCode.Joystick1Button5))
            {
                MainScreenUI.SetActive(true);

                Debug.Log("Right Bumper Released");
            }
        }
    }
}