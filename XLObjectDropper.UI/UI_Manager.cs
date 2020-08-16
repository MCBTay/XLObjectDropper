using UnityEngine;

namespace XLObjectDropper.UI
{
	public class UI_Manager : MonoBehaviour
	{
		public GameObject MainScreenUI;
		public GameObject RightBumperUI;

		public static UI_Manager Instance { get; private set; }

		private void Awake()
		{
			Instance = this;

			RightBumperUI.SetActive(false);
		}

		// Update is called once per frame
		private void Update()
		{
			if (Input.GetKey(KeyCode.Joystick1Button5))
			{
				MainScreenUI.SetActive(false);
				RightBumperUI.SetActive(true);

				Debug.Log("Right Bumper Pressed");
			}
			if (Input.GetKeyUp(KeyCode.Joystick1Button5))
			{
				MainScreenUI.SetActive(true);
				RightBumperUI.SetActive(false);

				Debug.Log("Right Bumper Released");
			}
		}
	}
}