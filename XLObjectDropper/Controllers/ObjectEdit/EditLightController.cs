using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using XLObjectDropper.UI.Controls;
using XLObjectDropper.UI.Controls.Buttons;
using XLObjectDropper.UI.Menus;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public static class EditLightController
	{
		static float ev100Min = 15.0f;
		static float ev100Max = 25.0f;
		static float numSteps = 40.0f;

		static float rangeMin = 0.0f;
		static float rangeMax = 20.0f;
		static float numRangeSteps = 80.0f;

		static float angleMin = 1.0f;
		static float angleMax = 179.0f;
		static float numAngleSteps = 178.0f;


		public static void AddLightOptions(GameObject SelectedObject, ObjectEditUI ObjectEdit)
		{
			var lights = SelectedObject.GetComponentsInChildren<Light>(true);

			if (lights != null && lights.Any())
			{
				var light = lights.First();
				var hdrpLight = light.GetComponent<HDAdditionalLightData>();

				var lightExpandable = ObjectEdit.AddLightSettings();

				var expandable = lightExpandable.GetComponent<LightSettingsExpandable>();

				expandable.EnabledToggle.onValueChanged += (isOn) =>
				{
					foreach (var lightToEdit in lights)
					{
						lightToEdit.GetComponent<HDAdditionalLightData>().enabled = isOn;
						lightToEdit.enabled = isOn;
					}
				};

				float currentIntensity = Mathf.Clamp(hdrpLight.intensity, ev100Min, ev100Max);
				float sliderValue = ((currentIntensity - ev100Min) / (ev100Max - ev100Min)) * numSteps;
				expandable.IntensitySlider.Slider.value = sliderValue;
				expandable.IntensitySlider.Value.SetText(sliderValue.ToString("N"));

				expandable.IntensitySlider.onValueChanged += (intensity) =>
				{
					foreach (var lightToEdit in lights)
					{
						var hdrpLightToEdit = lightToEdit.GetComponent<HDAdditionalLightData>();

						float newIntensity = ev100Min + ((intensity / numSteps) * (ev100Max - ev100Min));

						expandable.IntensitySlider.Value.SetText(newIntensity.ToString("N"));

						hdrpLightToEdit.SetIntensity(newIntensity, LightUnit.Ev100);
					}
				};

				float currentRange = Mathf.Clamp(hdrpLight.range, rangeMin, rangeMax);
				float rangeSliderValue = ((currentRange - rangeMin) / (rangeMax - rangeMin)) * numRangeSteps;
				expandable.RangeSlider.Slider.value = rangeSliderValue;
				expandable.RangeSlider.Value.SetText(rangeSliderValue.ToString("N"));

				expandable.RangeSlider.onValueChanged += (range) =>
				{
					foreach (var lightToEdit in lights)
					{
						float newRange = rangeMin + ((range / numRangeSteps) * (rangeMax - rangeMin));

						expandable.RangeSlider.Value.SetText(newRange.ToString("N"));

						lightToEdit.GetComponent<HDAdditionalLightData>().SetRange(newRange);
					}
				};

				if (hdrpLight.type != HDLightType.Spot)
				{
					expandable.AngleSlider.Slider.interactable = false;
				}
				else
				{
					float spotAngle = Mathf.Clamp(light.spotAngle, angleMin, angleMax);
					float angleSliderValue = ((spotAngle - angleMin) / (angleMax - angleMin)) * numAngleSteps;
					expandable.AngleSlider.Slider.value = angleSliderValue;
					expandable.AngleSlider.Value.SetText($"{(int)angleSliderValue}°");

					expandable.AngleSlider.onValueChanged += (angle) =>
					{
						foreach (var lightToEdit in lights)
						{
							float newSpotAngle = angleMin + (((angle - angleMin) / numAngleSteps) * (angleMax - angleMin));
							lightToEdit.spotAngle = newSpotAngle;

							expandable.AngleSlider.Value.SetText($"{(int)newSpotAngle}°");
						}
					};
				}

				expandable.ColorSliders.XSlider.Slider.value = hdrpLight.color.r;
				expandable.ColorSliders.XSlider.Value.SetText(hdrpLight.color.r.ToString("N"));

				expandable.ColorSliders.YSlider.Slider.value = hdrpLight.color.g;
				expandable.ColorSliders.YSlider.Value.SetText(hdrpLight.color.g.ToString("N"));

				expandable.ColorSliders.ZSlider.Slider.value = hdrpLight.color.b;
				expandable.ColorSliders.ZSlider.Value.SetText(hdrpLight.color.b.ToString("N"));

				expandable.ColorSliders.onValueChanged += (color) =>
				{
					foreach (var lightToEdit in lights)
					{
						lightToEdit.GetComponent<HDAdditionalLightData>().color = new Color(color.x, color.y, color.z);

						expandable.ColorSliders.XSlider.Value.SetText(color.x.ToString("N"));
						expandable.ColorSliders.YSlider.Value.SetText(color.y.ToString("N"));
						expandable.ColorSliders.ZSlider.Value.SetText(color.z.ToString("N"));
					}
				};

				//var nav = lightExpandable.GetComponent<Button>().navigation;
				//nav.mode = Navigation.Mode.Explicit;
				//nav.selectOnDown = toggle.GetComponent<ToggleControl>().Toggle;

				//lightExpandable.GetComponent<Selectable>().navigation = nav;
				//}
			}
		}

		private static void AddIntensitySlider(Expandable expandable, Light[] lights)
		{
			var light = lights.First();

			var hdrpLight = light.GetComponent<HDAdditionalLightData>();
			if (hdrpLight != null)
			{
				hdrpLight.SetLightUnit(LightUnit.Ev100);

				//float ev100Min = 15.0f;
				//float ev100Max = 25.0f;
				//float numSteps = 40.0f;

				float intensity = Mathf.Clamp(hdrpLight.intensity, ev100Min, ev100Max);

				float sliderValue = ((intensity - ev100Min) / (ev100Max - ev100Min)) * numSteps;

				expandable.AddSlider("Intensity", sliderValue, 0.0f, numSteps, true, value =>
				{
					foreach (var lightToEdit in lights)
					{
						var hdrpLightToEdit = lightToEdit.GetComponent<HDAdditionalLightData>();

						float newIntensity = ev100Min + ((value / ev100Max) * (ev100Max - ev100Min));

						hdrpLightToEdit.SetIntensity(newIntensity, LightUnit.Ev100);
					}
				});
			}
		}

		private static void AddRangeSlider(Expandable expandable, Light[] lights)
		{
			var light = lights.First();

			var hdrpLight = light.GetComponent<HDAdditionalLightData>();
			if (hdrpLight != null)
			{
				//float rangeMin = 0.0f;
				//float rangeMax = 20.0f;
				//float numSteps = 80.0f;

				float range = Mathf.Clamp(hdrpLight.range, rangeMin, rangeMax);

				float sliderValue = ((range - rangeMin) / (rangeMax - rangeMin)) * numSteps;

				expandable.AddSlider("Range", sliderValue, 0.0f, numRangeSteps, true, value =>
				{
					foreach (var lightToEdit in lights)
					{
						float newRange = rangeMin + ((value / rangeMax) * (rangeMax - rangeMin));

						lightToEdit.GetComponent<HDAdditionalLightData>().SetRange(newRange);
					}
				});
			}
		}

		private static void AddAngleSlider(Expandable expandable, Light[] lights)
		{
			var light = lights.First();

			var hdrpLight = light.GetComponent<HDAdditionalLightData>();

			if (hdrpLight.type != HDLightType.Spot) return;

			if (hdrpLight != null)
			{
				

				float spotAngle = Mathf.Clamp(light.spotAngle, rangeMin, rangeMax);

				float angle = ((spotAngle - angleMin) / (angleMax - angleMin)) * numAngleSteps;

				expandable.AddSlider("Angle", angle, 0.0f, numAngleSteps, true, value =>
				{
					foreach (var lightToEdit in lights)
					{
						float newSpotAngle = angleMin + ((value / angleMax) * (angleMax - angleMin));
						lightToEdit.spotAngle = newSpotAngle;
					}
				});
			}
		}

		private static void AddColorSliders(Expandable expandable, Light[] lights)
		{
			var light = lights.First();

			var hdrpLight = light.GetComponent<HDAdditionalLightData>();

			if (hdrpLight.type != HDLightType.Spot) return;

			if (hdrpLight != null)
			{
				float rangeMin = 0.0f;
				float rangeMax = 255.0f;
				float numSteps = 255.0f;

				expandable.AddVector3Slider("Color", "Red", "Green", "Blue", hdrpLight.color, value =>
				{
					foreach (var lightToEdit in lights)
					{
						lightToEdit.GetComponent<HDAdditionalLightData>().color = new Color(value.x, value.y, value.z);
					}
				});
			}
		}
	}
}
