using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls
{
	public class AXYBController : MonoBehaviour
	{
		[Header("A, X, Y, B Buttons")] 
		public GameObject AButton;
		public TMP_Text AButtonLabel;
		[HideInInspector] private bool AButtonEnabled { get; set; }
		public GameObject XButton;
		public TMP_Text XButtonLabel;
		[HideInInspector] private bool XButtonEnabled { get; set; }
		public GameObject YButton;
		public TMP_Text YButtonLabel;
		[HideInInspector] private bool YButtonEnabled { get; set; }
		public GameObject BButton;
		public TMP_Text BButtonLabel;
		[HideInInspector] private bool BButtonEnabled { get; set; }

		[Space(10)] 
		[Header("A, X, Y, B Buttons Pressed")]
		public GameObject AButtonPressed;
		public GameObject XButtonPressed;
		public GameObject YButtonPressed;
		public GameObject BButtonPressed;

		void OnEnable()
		{
			SetDefaultState();
		}

		void OnDisable()
		{
			SetDefaultState();
		}

		void SetDefaultState()
		{
			AButtonEnabled = BButtonEnabled = XButtonEnabled = YButtonEnabled = true;

			AButton.SetActive(true);
			XButton.SetActive(true);
			YButton.SetActive(true);
			BButton.SetActive(true);

			AButtonPressed.SetActive(false);
			XButtonPressed.SetActive(false);
			YButtonPressed.SetActive(false);
			BButtonPressed.SetActive(false);
		}

		void Start()
		{
			SetDefaultState();
		}

		// Update is called once per frame
		void Update()
		{
			var player = UIManager.Instance.Player;

			#region A Button
			if (AButtonEnabled)
			{
				if (player.GetButtonDown("A"))
				{
					AButton.SetActive(false);
					AButtonPressed.SetActive(true);
				}

				if (player.GetButtonUp("A"))
				{
					AButton.SetActive(true);
					AButtonPressed.SetActive(false);
				}
			}
			#endregion

			#region X Button
			if (XButtonEnabled)
			{
				if (player.GetButtonDown("X"))
				{
					XButton.SetActive(false);
					XButtonPressed.SetActive(true);
				}

				if (player.GetButtonUp("X"))
				{
					XButton.SetActive(true);
					XButtonPressed.SetActive(false);
				}
			}
			#endregion

			#region Y Button
			if (YButtonEnabled)
			{
				if (player.GetButtonDown("Y"))
				{
					YButton.SetActive(false);
					YButtonPressed.SetActive(true);
				}

				if (player.GetButtonUp("Y"))
				{
					YButton.SetActive(true);
					YButtonPressed.SetActive(false);
				}
			}
			#endregion

			#region B Button
			if (BButtonEnabled)
			{
				if (player.GetButtonDown("B"))
				{
					BButton.SetActive(false);
					BButtonPressed.SetActive(true);
				}

				if (player.GetButtonUp("B"))
				{
					BButton.SetActive(true);
					BButtonPressed.SetActive(false);
				}
			}
			#endregion
		}

		public void SetAButtonLabelText(string text, bool buttonEnabled = true)
		{
			AButtonEnabled = buttonEnabled;
			AButtonLabel.SetText(text);

			AButtonLabel.alpha = GetAlpha(buttonEnabled);
			AButton.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, GetAlpha(buttonEnabled));
		}

		public void SetBButtonLabelText(string text, bool buttonEnabled = true)
		{
			BButtonEnabled = buttonEnabled;
			BButtonLabel.SetText(text);

			BButtonLabel.alpha = GetAlpha(buttonEnabled);
			BButton.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, GetAlpha(buttonEnabled));
		}

		public void SetXButtonLabelText(string text, bool buttonEnabled = true)
		{
			XButtonEnabled = buttonEnabled;
			XButtonLabel.SetText(text);

			XButtonLabel.alpha = GetAlpha(buttonEnabled);
			XButton.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, GetAlpha(buttonEnabled));
		}

		public void SetYButtonLabelText(string text, bool buttonEnabled = true)
		{
			YButtonEnabled = buttonEnabled;
			YButtonLabel.SetText(text);

			YButtonLabel.alpha = GetAlpha(buttonEnabled);
			YButton.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, GetAlpha(buttonEnabled));
		}

		private float GetAlpha(bool enabled) { return enabled ? 1.0f : 0.3f; }
	}
}
