using TMPro;
using UnityEngine;

namespace XLObjectDropper.UI.Controls
{
	public class StickController : MonoBehaviour
	{
		public GameObject StickButton;
		public GameObject StickButtonPressed;
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
	}
}
