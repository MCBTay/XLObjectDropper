using UnityEngine;

public class UI_Manager : MonoBehaviour
{
	public GameObject MainScreenUI;
	public GameObject RightBumperUI;

	private void Start()
	{
		RightBumperUI.SetActive(false);
	}

	// Update is called once per frame
	void Update()
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