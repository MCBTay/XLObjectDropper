using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls
{
	public class TriggerButton : MonoBehaviour
	{
		public GameObject Trigger;
		public GameObject TriggerPressed;
		public TMP_Text TriggerLabel;
		public string ButtonName;
		[HideInInspector] private bool TriggerEnabled;

		private void Update()
		{
			var player = UIManager.Instance.Player;

			if (TriggerEnabled)
			{
				if (player.GetButton(ButtonName))
				{
					Trigger.SetActive(false);
					TriggerPressed.SetActive(true);
				}
				else
				{
					Trigger.SetActive(true);
					TriggerPressed.SetActive(false);
				}
			}
			else
			{
				Trigger.SetActive(true);
				TriggerPressed.SetActive(false);
			}
		}

		public void EnableTrigger(bool buttonEnabled)
		{
			TriggerEnabled = buttonEnabled;

			TriggerLabel.alpha = Utilities.Color.GetAlpha(buttonEnabled);
			Trigger.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, Utilities.Color.GetAlpha(buttonEnabled));
		}
	}
}
