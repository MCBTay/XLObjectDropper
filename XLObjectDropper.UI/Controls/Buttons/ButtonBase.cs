using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XLObjectDropper.UI.Utilities;
using Color = UnityEngine.Color;

namespace XLObjectDropper.UI.Controls.Buttons
{
	public class ButtonBase : MonoBehaviour
	{
		public GameObject Button;
		public Image ButtonImage;
		public GameObject ButtonPressed;
		public Image ButtonPressedImage;
		public TMP_Text ButtonLabel;
		public ControllerButton ControllerButton;

		[Header("Xbox Sprites")]
		public Sprite Xbox;
		public Sprite XboxPressed;
		[Header("Playstation Sprites")]
		public Sprite Playstation;
		public Sprite PlaystationPressed;
		[Header("Switch Sprites")]
		public Sprite Switch;
		public Sprite SwitchPressed;

		[HideInInspector] protected bool ButtonEnabled;


		private void Start()
		{
			SetButtonSprites();
			SetDefaultState();
		}

		private void SetButtonSprites()
		{
			if (Xbox == null || XboxPressed == null) return;

			switch (PlatformHelper.GetPlatformType())
			{
				case PlatformType.Playstation:
					ButtonImage.sprite = Playstation;
					ButtonPressedImage.sprite = PlaystationPressed;
					break;
				case PlatformType.Switch:
					ButtonImage.sprite = Switch;
					ButtonPressedImage.sprite = SwitchPressed;
					break;
				case PlatformType.Xbox:
				default:
					ButtonImage.sprite = Xbox;
					ButtonPressedImage.sprite = XboxPressed;
					break;
			}
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
				if (player.GetButton(ControllerButton.ToString().Replace('_', ' ')))
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
