using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XLObjectDropper.UI.Controls.Expandables;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities.Save;
using XLObjectDropper.Utilities.Save.Settings;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public class EditColorTintController : IObjectSettings
	{
		private static EditColorTintController _instance;
		public static EditColorTintController Instance => _instance ?? (_instance = new EditColorTintController());

		private Color? Color;

		public void AddOptions(GameObject SelectedObject, ObjectEditUI ObjectEdit)
		{
			var material = SelectedObject?.GetComponentInChildren<MeshRenderer>()?.material;
			if (material == null) return;

			var settings = ObjectEdit.AddColorTintSettings();

			var expandable = settings.GetComponent<Expandable>();
			var colorTintExpandable = settings.GetComponent<ColorTintExpandable>();

			if (colorTintExpandable == null) return;

			var buttons = expandable.PropertiesListContent.GetComponentsInChildren<Button>();

			foreach (var button in buttons)
			{
				button.onClick.AddListener(() => ColorClicked(SelectedObject, button.gameObject.GetComponent<Image>().color));
			}
		}

		private void ColorClicked(GameObject gameObject, Color color)
		{
			Color = color;

			var meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

			foreach (var meshRenderer in meshRenderers)
			{
				meshRenderer.material.SetColor("_BaseColor", color);
			}
		}

		public ISettingsSaveData ConvertToSaveSettings()
		{
			if (Color == null) return null;

			return new ColorTintSaveData
			{
				tintedColor = new SerializableVector3(Color.Value.r, Color.Value.g, Color.Value.b)
			};
		}

		public void ApplySaveSettings(GameObject selectedObject, List<ISettingsSaveData> settings)
		{
			if (settings == null || !settings.Any()) return;

			var colorTintSettings = settings.OfType<ColorTintSaveData>().ToList();
			if (!colorTintSettings.Any()) return;

			var colorTintSetting = colorTintSettings.First();

			Color = new Color(colorTintSetting.tintedColor.x, colorTintSetting.tintedColor.y, colorTintSetting.tintedColor.z);

			var meshRenderers = selectedObject.GetComponentsInChildren<MeshRenderer>(true);
			foreach (var meshRenderer in meshRenderers)
			{
				meshRenderer.material.SetColor("_BaseColor", Color.Value);
			}
		}
	}
}
