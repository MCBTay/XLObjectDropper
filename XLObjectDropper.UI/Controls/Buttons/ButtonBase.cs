using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls.Buttons
{
	public class ButtonBase : MonoBehaviour
	{
		public GameObject Button;
		public GameObject ButtonPressed;
		public TMP_Text ButtonLabel;
		public string ButtonName;
		[HideInInspector] protected bool ButtonEnabled;

		private void Start()
		{
			SetDefaultState();
		}

		public void SetDefaultState()
		{
			ButtonEnabled = true;
			Button.SetActive(true);
			ButtonPressed.SetActive(false);
		}

		private void Update()
		{
			var player = UIManager.Instance.Player;

			if (ButtonEnabled)
			{
				if (player.GetButton(ButtonName))
				{
					Button.SetActive(false);
					ButtonPressed.SetActive(true);
				}
				else
				{
					Button.SetActive(true);
					ButtonPressed.SetActive(false);
				}
			}
			else
			{
				Button.SetActive(true);
				ButtonPressed.SetActive(false);
			}
		}

		public void EnableButton(bool buttonEnabled)
		{
			ButtonEnabled = buttonEnabled;

			ButtonLabel.alpha = Utilities.Color.GetAlpha(buttonEnabled);
			Button.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, Utilities.Color.GetAlpha(buttonEnabled));
		}
	}
}
