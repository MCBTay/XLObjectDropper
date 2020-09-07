using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.UI;

namespace XLObjectDropper
{
	public class Spawnable
	{
		public GameObject Prefab;
		public Texture2D PreviewTexture;
		public string BundleName { get; set; }

		public Spawnable(GameObject prefab, AssetBundle bundle)
		{
			Prefab = prefab;
			BundleName = bundle.name;
			
			PlayerController.Instance.StartCoroutine(LoadPreviewImage());
		}

		private IEnumerator LoadPreviewImage()
		{
			var imagesPath = Path.Combine(Main.ModPath, "Images");

			DirectorySecurity securityRules = new DirectorySecurity();
			securityRules.AddAccessRule(new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow));

			if (!Directory.Exists(imagesPath)) Directory.CreateDirectory(imagesPath, securityRules);

			var bundlePath = Path.Combine(imagesPath, BundleName);
			if (!Directory.Exists(bundlePath)) Directory.CreateDirectory(bundlePath, securityRules);

			var filePath = Path.Combine(bundlePath, Prefab.name + ".png");

			Texture2D texture;

			if (File.Exists(filePath))
			{
				var fileData = File.ReadAllBytes(filePath);
				yield return null;

				texture = new Texture2D(2, 2);
				texture.LoadImage(fileData);
				yield return null;
			}

			RuntimePreviewGenerator.MarkTextureNonReadable = false;
			RuntimePreviewGenerator.OrthographicMode = false;
			//RuntimePreviewGenerator.BackgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
			PreviewTexture = RuntimePreviewGenerator.GenerateModelPreview(Prefab.transform, 128, 128, true);
			yield return null;

			File.WriteAllBytes(bundlePath, PreviewTexture.EncodeToPNG());
		}
	}
}
