using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls
{
	public class StickController : MonoBehaviour
	{
		public GameObject StickButton;
		public GameObject StickButtonPressed;
		[HideInInspector] private bool StickButtonEnabled;
		public TMP_Text StickButtonLabel;
		public GameObject StickAnimatedBase;
		public GameObject StickAnimatedOverlay;
		public TMP_Text StickAnimatedLabel;
		public string XAxisName;
		public string YAxisName;
		public string ButtonName;


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

		private void SetDefaultState()
		{
			StickButton.SetActive(true);
			StickButtonPressed.SetActive(false);
		}

		private void Update()
		{
			var player = UIManager.Instance.Player;

			var stick = player.GetAxis2D(XAxisName, YAxisName);
			var scaleFactor = 2.0f;
			StickAnimatedOverlay.transform.localPosition = new Vector3(stick.x * scaleFactor, stick.y * scaleFactor, 0.0f);

			if (StickButtonEnabled)
			{
				if (player.GetButtonDown(ButtonName))
				{
					StickButton.SetActive(false);
					StickButtonPressed.SetActive(true);
				}

				if (player.GetButtonUp(ButtonName))
				{
					StickButton.SetActive(true);
					StickButtonPressed.SetActive(false);
				}
			}
			else
			{
				StickButton.SetActive(true);
				StickButtonPressed.SetActive(false);
			}
		}

		public void EnableStickButton(bool buttonEnabled)
		{
			StickButtonEnabled = buttonEnabled;

			StickButtonLabel.alpha = Utilities.Color.GetAlpha(buttonEnabled);
			StickButton.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, Utilities.Color.GetAlpha(buttonEnabled));
		}
	}
}
