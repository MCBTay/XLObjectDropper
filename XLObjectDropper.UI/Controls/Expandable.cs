using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls
{
	public class Expandable : MonoBehaviour, ISelectHandler, IDeselectHandler
	{
		public GameObject ExpandableItem;
		public GameObject Properties;
		public Animator Animator;
		public Button Button;
		[HideInInspector] private bool Expanded;
		[HideInInspector] public UnityEvent onSelect;

		public GameObject SliderPrefab;
		public GameObject TogglePrefab;

		private void OnEnable()
		{
			//Button.onClick.AddListener(() =>
			//{
				
			//});
		}

		private void OnDisable()
		{
			Button.onClick.RemoveAllListeners();
		}

		public void Update()
		{
			GetComponent<Outline>().enabled = false;

			if (ExpandableItem == EventSystem.current.currentSelectedGameObject)
			{
				GetComponent<Outline>().enabled = true;
			}
		}

		public void AddToggle(string text, bool isChecked)
		{
			var toggle = Instantiate(TogglePrefab, Properties.transform);

			var toggleControl = toggle.GetComponent<ToggleControl>();

			toggleControl.Label.SetText(text);
			toggleControl.Toggle.isOn = isChecked;

			//if (objectClicked != null)
			//{
			//	listItem.GetComponent<Button>().onClick.AddListener(objectClicked);
			//}

			//if (objectSelected != null)
			//{
			//	listItem.GetComponent<ObjectSelectionListItem>().onSelect.AddListener(objectSelected);
			//}

			toggle.SetActive(true);
		}

		public void AddSlider(string text, float value, float minValue, float maxValue, bool wholeNumbers, UnityAction<float> onValueChanged = null)
		{
			var slider = Instantiate(SliderPrefab, Properties.transform);

			var sliderControl = slider.GetComponent<SliderControl>();

			sliderControl.Label.SetText(text);

			sliderControl.Slider.wholeNumbers = wholeNumbers;

			sliderControl.Slider.minValue = minValue;
			sliderControl.Slider.maxValue = maxValue;

			sliderControl.Slider.value = Mathf.Clamp(value, minValue, maxValue);

			//if (objectClicked != null)
			//{
			//	listItem.GetComponent<Button>().onClick.AddListener(objectClicked);
			//}

			if (onValueChanged != null)
			{
				sliderControl.onValueChanged += onValueChanged;
			}

			slider.SetActive(true);
		}

		public void OnSelect(BaseEventData eventData)
		{
			Animator.Play("Expand");

			Expanded = true;
			Properties.SetActive(true);
		}

		public void OnDeselect(BaseEventData eventData)
		{
			Animator.Play("Collapse");

			Expanded = false;
			Properties.SetActive(false);
		}
	}
}
