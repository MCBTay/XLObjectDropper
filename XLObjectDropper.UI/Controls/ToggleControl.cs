using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls
{
	public class ToggleControl : MonoBehaviour
	{
		public Toggle Toggle;
		public TMP_Text Label;
		[HideInInspector] public UnityAction<bool> onValueChanged = (x) => { };

		private void OnEnable()
		{
			Toggle.onValueChanged.AddListener((value) =>
			{
				onValueChanged.Invoke(value);
				Toggle.isOn = value;
			});
		}

		private void OnDisable()
		{
			Toggle.onValueChanged.RemoveAllListeners();
		}
	}
}
