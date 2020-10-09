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

			XSlider.Slider.onValueChanged.AddListener(action);
			YSlider.Slider.onValueChanged.AddListener(action);
			ZSlider.Slider.onValueChanged.AddListener(action);
		}

		public void OnDestroy()
		{
			XSlider.Slider.onValueChanged.RemoveAllListeners();
			YSlider.Slider.onValueChanged.RemoveAllListeners();
			ZSlider.Slider.onValueChanged.RemoveAllListeners();
		}
	}
}
