using System;
using UnityEngine;
using UnityEngine.UI;
using XLObjectDropper.UI.Controls.Expandables;
using XLObjectDropper.UI.Menus;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public class EditColorTintController : IObjectSettings
	{
		private static EditColorTintController _instance;
		public static EditColorTintController Instance => _instance ?? (_instance = new EditColorTintController());

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
			var meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();

			foreach (var meshRenderer in meshRenderers)
			{
				meshRenderer.material.SetColor("_BaseColor", color);
			}
		}
	}
}
