using System.Collections;
using System.IO;
using System.Threading.Tasks;
using RealisticEyeMovements;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

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

			LoadPreviewImage();
		}

		private void LoadPreviewImage()
		{
			var filePath = Path.Combine(AssetBundleHelper.ImagesPath, BundleName, Prefab.name + ".png");

			if (File.Exists(filePath))
			{
				var fileData = File.ReadAllBytes(filePath);

				var texture = new Texture2D(2, 2);
				texture.LoadImage(fileData);
			}
			else
			{
				PlayerController.Instance.StartCoroutine(GeneratePreviewImage());
			}
		}

		private void LoadPreviewImageSync()
		{
			var imagesPath = Path.Combine(Main.ModPath, "Images");

			if (!Directory.Exists(imagesPath)) Directory.CreateDirectory(imagesPath);

			var bundlePath = Path.Combine(imagesPath, BundleName);
			if (!Directory.Exists(bundlePath)) Directory.CreateDirectory(bundlePath);

			var filePath = Path.Combine(bundlePath, Prefab.name + ".png");

			Texture2D texture;

			if (File.Exists(filePath))
			{
				var fileData = File.ReadAllBytes(filePath);

				texture = new Texture2D(2, 2);
				texture.LoadImage(fileData);
			}


			RuntimePreviewGenerator.MarkTextureNonReadable = false;
			RuntimePreviewGenerator.OrthographicMode = true;
			//RuntimePreviewGenerator.BackgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
			//RuntimePreviewGenerator.PreviewRenderCamera = Camera.Instantiate(AssetBundleHelper.SelfieCamera);
			PreviewTexture = RuntimePreviewGenerator.GenerateModelPreview(Prefab.transform, 128, 128, true);

			File.WriteAllBytes(Path.Combine(Main.ModPath, Prefab.name + ".png"), PreviewTexture.EncodeToPNG());
		}

		private IEnumerator GeneratePreviewImage()
		{
			yield return new WaitForEndOfFrame();

			RuntimePreviewGenerator.PreviewRenderCamera = Object.Instantiate(AssetBundleHelper.SelfieCamera);
			RuntimePreviewGenerator.MarkTextureNonReadable = false;
			RuntimePreviewGenerator.OrthographicMode = true;
			RuntimePreviewGenerator.BackgroundColor = new Color(0.3f, 0.3f, 0.3f, 1);

			PreviewTexture = RuntimePreviewGenerator.GenerateModelPreview(Prefab.transform, 128, 128);

			RuntimePreviewGenerator.PreviewRenderCamera.enabled = false;
			Object.DestroyImmediate(RuntimePreviewGenerator.PreviewRenderCamera.gameObject, true);
			//File.WriteAllBytes(Path.Combine(Main.ModPath, Prefab.name + ".png"), PreviewTexture.EncodeToPNG());
			WriteImage(Path.Combine(Main.ModPath, Prefab.name + ".png"), PreviewTexture);
		}

		async Task WriteImage(string filePath, Texture2D texture)
		{
			byte[] image = texture.EncodeToPNG();

			using (FileStream sourceStream = new FileStream(filePath,
				FileMode.Append, FileAccess.Write, FileShare.None,
				bufferSize: 4096, useAsync: true))
			{
				await sourceStream.WriteAsync(image, 0, image.Length);
			};
		}
	}
}
