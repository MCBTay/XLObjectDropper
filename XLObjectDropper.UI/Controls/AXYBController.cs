using TMPro;
using UnityEngine;

namespace XLObjectDropper.UI.Controls
{
	public class AXYBController : MonoBehaviour
	{
		[Header("A, X, Y, B Buttons")] 
		public GameObject AButton;
		public TMP_Text AButtonLabel;
		public GameObject XButton;
		public TMP_Text XButtonLabel;
		public GameObject YButton;
		public TMP_Text YButtonLabel;
		public GameObject BButton;
		public TMP_Text BButtonLabel;

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

			#endregion

			#region X Button

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

			#endregion

			#region Y Button

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

			#endregion

			#region B Button

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

			#endregion
		}

		public void SetAButtonLabelText(string text)
		{
			AButtonLabel.SetText(text);
		}

		public void SetBButtonLabelText(string text)
		{
			BButtonLabel.SetText(text);
		}

		public void SetXButtonLabelText(string text)
		{
			XButtonLabel.SetText(text);
		}

		public void SetYButtonLabelText(string text)
		{
			YButtonLabel.SetText(text);
		}
	}
}
