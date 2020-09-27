using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls
{
	public class SliderControl : MonoBehaviour
	{
		public Slider Slider;
		public TMP_Text Label;
		public TMP_Text Value;
		[HideInInspector] public event UnityAction<float> onValueChanged = (x) => { };

		private void OnAwake()
		{
			Slider.onValueChanged.AddListener((value) =>
			{
				onValueChanged.Invoke(value);
				Value.SetText(value.ToString("N"));
			});
		}

		private void OnEnable()
		{
			Slider.onValueChanged.RemoveAllListeners();

			Slider.onValueChanged.AddListener((value) =>
			{
				onValueChanged.Invoke(value);
				Value.SetText(value.ToString("N"));
			});
		}

		private void OnDisable()
		{
			Slider.onValueChanged.RemoveAllListeners();
		}
	}
}
