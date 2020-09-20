using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls
{
	public class AXYBButton : MonoBehaviour
	{
		public GameObject Button;
		public TMP_Text ButtonLabel;
		public GameObject ButtonPressed;
		[HideInInspector] public bool ButtonEnabled { get; set; }

		private void OnEnable()
		{
			SetDefaultState();
		}

		private void OnDisable()
		{
			SetDefaultState();
		}

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

		public void UpdateButton(string text, bool buttonEnabled = true)
		{
			if (!string.IsNullOrEmpty(text))
				SetLabelText(text);

			SetButtonEnabled(buttonEnabled);
		}

		public void SetLabelText(string text)
		{
			ButtonLabel.SetText(text);
		}

		public void SetButtonEnabled(bool buttonEnabled)
		{
			ButtonEnabled = buttonEnabled;

			ButtonLabel.alpha = GetAlpha(buttonEnabled);
			Button.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, GetAlpha(buttonEnabled));
		}

		public void ToggleButtonSprite(bool pressed)
		{
			Button.SetActive(!pressed);
			ButtonPressed.SetActive(pressed);
		}

		private float GetAlpha(bool buttonEnabled) { return buttonEnabled ? 1.0f : 0.3f; }
	}
}
