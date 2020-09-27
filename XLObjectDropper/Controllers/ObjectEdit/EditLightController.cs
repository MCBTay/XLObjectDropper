using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityModManagerNet;
using XLObjectDropper.UI.Controls;
using XLObjectDropper.UI.Menus;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public static class EditLightController
	{
		public static void AddLightOptions(GameObject SelectedObject, ObjectEditUI ObjectEdit)
		{
			var lights = SelectedObject.GetComponentsInChildren<Light>(true);

			if (lights != null && lights.Any())
			{
				var lightExpandable = ObjectEdit.AddToList("Light");

				var expandable = lightExpandable.GetComponent<Expandable>();

				if (expandable != null)
				{
					expandable.AddToggle("Enabled", true, (isOn) =>
					{
						foreach (var lightToEdit in lights)
						{
							var hdrpLight = lightToEdit.GetComponent<HDAdditionalLightData>();
							UnityModManager.Logger.Log("Setting light enabled to " + isOn);
							hdrpLight.enabled = isOn;
							lightToEdit.enabled = isOn;
						}
					});

					AddIntensitySlider(expandable, lights);
					AddRangeSlider(expandable, lights);
					AddAngleSlider(expandable, lights);
					AddColorSliders(expandable, lights);
				}
			}
		}

		private static void AddIntensitySlider(Expandable expandable, Light[] lights)
		{
			var light = lights.First();

			var hdrpLight = light.GetComponent<HDAdditionalLightData>();
			if (hdrpLight != null)
			{
				hdrpLight.SetLightUnit(LightUnit.Ev100);

				float ev100Min = 15.0f;
				float ev100Max = 25.0f;
				float numSteps = 40.0f;

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
				float rangeMin = 0.0f;
				float rangeMax = 20.0f;
				float numSteps = 80.0f;

				float range = Mathf.Clamp(hdrpLight.range, rangeMin, rangeMax);

				float sliderValue = ((range - rangeMin) / (rangeMax - rangeMin)) * numSteps;

				expandable.AddSlider("Range", sliderValue, 0.0f, numSteps, true, value =>
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
				float rangeMin = 1.0f;
				float rangeMax = 179.0f;
				float numSteps = 179.0f;

				float spotAngle = Mathf.Clamp(light.spotAngle, rangeMin, rangeMax);

				float angle = ((spotAngle - rangeMin) / (rangeMax - rangeMin)) * numSteps;

				expandable.AddSlider("Angle", angle, 0.0f, numSteps, true, value =>
				{
					foreach (var lightToEdit in lights)
					{
						float newSpotAngle = rangeMin + ((value / rangeMax) * (rangeMax - rangeMin));
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
