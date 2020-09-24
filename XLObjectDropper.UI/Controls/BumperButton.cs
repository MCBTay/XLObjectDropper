using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls
{
	public class BumperButton : MonoBehaviour
	{
		public GameObject Bumper;
		public GameObject BumperPressed;
		public TMP_Text BumperLabel;
		public string ButtonName;
		[HideInInspector] private bool BumperEnabled;

		private void Update()
		{
			var player = UIManager.Instance.Player;
			if (BumperEnabled)
			{
				if (player.GetButton(ButtonName))
				{
					Bumper.SetActive(false);
					BumperPressed.SetActive(true);
				}
				else
				{
					Bumper.SetActive(true);
					BumperPressed.SetActive(false);
				}
			}
			else
			{
				Bumper.SetActive(true);
				BumperPressed.SetActive(false);
			}
		}

		public void EnableBumper(bool buttonEnabled)
		{
			BumperEnabled = buttonEnabled;

			BumperLabel.alpha = Utilities.Color.GetAlpha(buttonEnabled);
			Bumper.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, Utilities.Color.GetAlpha(buttonEnabled));
		}
	}
}
