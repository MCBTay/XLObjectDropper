using TMPro;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls
{
	public class ToggleControl : SelectableControl
	{
		public Toggle Toggle;
		public TMP_Text Label;

		private void OnEnable()
		{
			Toggle.onValueChanged.AddListener(ToggleValueChanged);
		}

		private void ToggleValueChanged(bool value)
		{
			Toggle.isOn = value;
		}

		private void OnDisable()
		{
			Toggle.onValueChanged.RemoveListener(ToggleValueChanged);
		}
	}
}
