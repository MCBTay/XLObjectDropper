using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using XLObjectDropper.UI.Controls.Expandables;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities;
using XLObjectDropper.Utilities.Save;
using XLObjectDropper.Utilities.Save.Settings;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public class EditLightController : IObjectSettings
	{
		private float ev100Min = 15.0f;
		private float ev100Max = 40.0f;
		private float numSteps = 100.0f;

		private float rangeMin = 0.0f;
		private float rangeMax = 20.0f;
		private float numRangeSteps = 80.0f;

		private float angleMin = 1.0f;
		private float angleMax = 179.0f;
		private float numAngleSteps = 178.0f;

		public bool Enabled;
		public float Intensity;
		public LightUnit Unit;
		public float Range;
		public float? Angle;
		public Vector3 Color;

		public void AddOptions(GameObject SelectedObject, ObjectEditUI ObjectEdit)
		{
			var lights = SelectedObject.GetComponentsInChildren<Light>(true);

			if (lights != null && lights.Any())
			{
				var light = lights.First();
				var hdrpLight = light.GetComponent<HDAdditionalLightData>();

				var lightExpandable = ObjectEdit.AddLightSettings();

				var expandable = lightExpandable.GetComponent<LightSettingsExpandable>();

				Enabled = light.enabled;

				expandable.EnabledToggle.Toggle.onValueChanged.AddListener((isOn) =>
				{
					Enabled = isOn;
					foreach (var lightToEdit in lights)
					{
						lightToEdit.GetComponent<HDAdditionalLightData>().enabled = isOn;
						lightToEdit.enabled = isOn;
					}
				});

				float currentIntensity = Mathf.Clamp(hdrpLight.intensity, ev100Min, ev100Max);
				float sliderValue = ((currentIntensity - ev100Min) / (ev100Max - ev100Min)) * numSteps;
				Intensity = sliderValue;
				Unit = LightUnit.Ev100;
				expandable.IntensitySlider.Slider.value = sliderValue;

				expandable.IntensitySlider.Slider.onValueChanged.AddListener((intensity) =>
				{
					foreach (var lightToEdit in lights)
					{
						var hdrpLightToEdit = lightToEdit.GetComponent<HDAdditionalLightData>();

						float newIntensity = ev100Min + ((intensity / numSteps) * (ev100Max - ev100Min));

						expandable.IntensitySlider.Value.SetText(newIntensity.ToString("N"));
						Intensity = newIntensity;
						Unit = LightUnit.Ev100;
						hdrpLightToEdit.SetIntensity(newIntensity, LightUnit.Ev100);
					}
				});

				float currentRange = Mathf.Clamp(hdrpLight.range, rangeMin, rangeMax);
				Range = currentRange;
				float rangeSliderValue = ((currentRange - rangeMin) / (rangeMax - rangeMin)) * numRangeSteps;
				expandable.RangeSlider.Slider.value = rangeSliderValue;
				expandable.RangeSlider.Value.SetText(rangeSliderValue.ToString("N"));

				expandable.RangeSlider.Slider.onValueChanged.AddListener((range) =>
				{
					foreach (var lightToEdit in lights)
					{
						float newRange = rangeMin + ((range / numRangeSteps) * (rangeMax - rangeMin));
						Range = newRange;
						expandable.RangeSlider.Value.SetText(newRange.ToString("N"));

						lightToEdit.GetComponent<HDAdditionalLightData>().SetRange(newRange);
					}
				});

				if (hdrpLight.type != HDLightType.Spot)
				{
					Angle = null;
					expandable.AngleSlider.Slider.interactable = false;
				}
				else
				{
					float spotAngle = Mathf.Clamp(light.spotAngle, angleMin, angleMax);
					Angle = spotAngle;
					float angleSliderValue = ((spotAngle - angleMin) / (angleMax - angleMin)) * numAngleSteps;
					expandable.AngleSlider.Slider.value = angleSliderValue;
					expandable.AngleSlider.Value.SetText($"{(int)angleSliderValue}°");

					expandable.AngleSlider.Slider.onValueChanged.AddListener((angle) =>
					{
						foreach (var lightToEdit in lights)
						{
							float newSpotAngle = angleMin + (((angle - angleMin) / numAngleSteps) * (angleMax - angleMin));
							lightToEdit.spotAngle = newSpotAngle;
							Angle = newSpotAngle;
							expandable.AngleSlider.Value.SetText($"{(int)newSpotAngle}°");
						}
					});
				}

				Color = new Vector3(hdrpLight.color.r, hdrpLight.color.g, hdrpLight.color.b);

				expandable.ColorSliders.XSlider.Slider.value = hdrpLight.color.r;
				expandable.ColorSliders.XSlider.Value.SetText(hdrpLight.color.r.ToString("N"));

				expandable.ColorSliders.YSlider.Slider.value = hdrpLight.color.g;
				expandable.ColorSliders.YSlider.Value.SetText(hdrpLight.color.g.ToString("N"));

				expandable.ColorSliders.ZSlider.Slider.value = hdrpLight.color.b;
				expandable.ColorSliders.ZSlider.Value.SetText(hdrpLight.color.b.ToString("N"));

				expandable.ColorSliders.onValueChanged += (color) =>
				{
					Color = color;
					foreach (var lightToEdit in lights)
					{
						lightToEdit.GetComponent<HDAdditionalLightData>().color = new Color(color.x, color.y, color.z);

						expandable.ColorSliders.XSlider.Value.SetText(color.x.ToString("N"));
						expandable.ColorSliders.YSlider.Value.SetText(color.y.ToString("N"));
						expandable.ColorSliders.ZSlider.Value.SetText(color.z.ToString("N"));
					}
				};
			}
		}

		public ISettingsSaveData ConvertToSaveSettings()
		{
			return new LightingSaveData
			{
				enabled = Enabled,
				intensity = Intensity,
				unit = Unit,
				range = Range,
				angle = Angle,
				color = new SerializableVector3(Color)
			};
		}

		public void ApplySaveSettings(GameObject selectedObject, List<ISettingsSaveData> settings)
		{
			if (settings == null || !settings.Any()) return;

			var lightSettings = settings.OfType<LightingSaveData>().ToList();
			if (!lightSettings.Any()) return;

			var lightSetting = lightSettings.First();

			Enabled = lightSetting.enabled;
			Intensity = lightSetting.intensity;
			Unit = lightSetting.unit;
			Range = lightSetting.range;
			Angle = lightSetting.angle;
			Color = lightSetting.color;

			var lights = selectedObject.GetComponentsInChildren<Light>(true);
			if (lights == null || !lights.Any()) return;

			foreach (var light in lights)
			{
				var hdLight = light.GetComponent<HDAdditionalLightData>();

				light.enabled = Enabled;
				hdLight.enabled = Enabled;
				hdLight.lightUnit = Unit;
				hdLight.intensity = Intensity;
				hdLight.range = Range;
				if (Angle != null)
				{
					hdLight.SetSpotAngle(Angle.Value);
				}

				hdLight.color = new Color(Color.x, Color.y, Color.z);
			}
		}
	}
}
