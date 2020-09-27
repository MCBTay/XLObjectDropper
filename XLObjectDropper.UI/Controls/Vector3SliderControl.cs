using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace XLObjectDropper.UI.Controls
{
	public class Vector3SliderControl : MonoBehaviour
	{
		public TMP_Text Label;
		public SliderControl XSlider;
		public SliderControl YSlider;
		public SliderControl ZSlider;

		[HideInInspector] public event UnityAction<Vector3> onValueChanged = v => { };

		public void Awake()
		{
			UnityAction<float> action = v => {
				onValueChanged.Invoke(new Vector3(XSlider.Slider.value, YSlider.Slider.value, ZSlider.Slider.value));
			};

			XSlider.onValueChanged += action;
			YSlider.onValueChanged += action;
			ZSlider.onValueChanged += action;
		}
	}
}
