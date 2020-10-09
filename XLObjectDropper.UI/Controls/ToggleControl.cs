using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls
{
	public class ToggleControl : MonoBehaviour
	{
		public Toggle Toggle;
		public TMP_Text Label;

		private void OnEnable()
		{
			Toggle.onValueChanged.AddListener((value) =>
			{
				Toggle.isOn = value;
			});
		}

		private void OnDisable()
		{
			Toggle.onValueChanged.RemoveAllListeners();
		}
	}
}
