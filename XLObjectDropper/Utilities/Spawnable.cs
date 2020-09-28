using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using XLObjectDropper.UI.Utilities;

namespace XLObjectDropper.Utilities
{
	public class Spawnable
	{
		public SpawnableType Type;
		public GameObject Prefab;
		public GameObject SpawnedInstance;
		public Texture2D PreviewTexture;
		public string BundleName;
		public List<Spawnable> AlternateStyles;

		public Spawnable(SpawnableType type, GameObject prefab, AssetBundle bundle)
		{
			Type = type;
			Prefab = prefab;
			BundleName = bundle.name;

			LoadPreviewImage();
		}

		public Spawnable(SpawnableType type, GameObject prefab, AssetBundle bundle, List<GameObject> alternateStyles) : this(type, prefab, bundle)
		{
			AlternateStyles = new List<Spawnable>();
			foreach (var altStyle in alternateStyles)
			{
				AlternateStyles.Add(new Spawnable(type, altStyle, bundle));
			}
		}

		public Spawnable(GameObject prefab, GameObject spawnedInstance, Texture2D previewTexture)
		{
			Prefab = prefab;
			SpawnedInstance = spawnedInstance;
			PreviewTexture = previewTexture;
		}

		private void LoadPreviewImage()
		{
			var filePath = Path.Combine(AssetBundleHelper.ImagesPath, BundleName, Prefab.name + ".png");

			if (File.Exists(filePath))
			{
				var fileData = File.ReadAllBytes(filePath);

				PreviewTexture = new Texture2D(2, 2);
				PreviewTexture.LoadImage(fileData);
			}
			else
			{
				PlayerController.Instance.StartCoroutine(GeneratePreviewImage());
			}
		}

		private IEnumerator GeneratePreviewImage()
		{
			yield return new WaitForEndOfFrame();

			RuntimePreviewGenerator.PreviewRenderCamera = Object.Instantiate(AssetBundleHelper.SelfieCamera);
			RuntimePreviewGenerator.PreviewRenderCamera.enabled = true;

			RuntimePreviewGenerator.MarkTextureNonReadable = false;
			RuntimePreviewGenerator.OrthographicMode = true;

			PreviewTexture = RuntimePreviewGenerator.GenerateModelPreview(Prefab.transform, 128, 128);

			RuntimePreviewGenerator.PreviewRenderCamera.enabled = false;
			Object.DestroyImmediate(RuntimePreviewGenerator.PreviewRenderCamera.gameObject, true);

			var imagesPath = Path.Combine(Main.ModPath, "Images");

			if (!Directory.Exists(imagesPath)) Directory.CreateDirectory(imagesPath);

			var bundlePath = Path.Combine(imagesPath, BundleName);
			if (!Directory.Exists(bundlePath)) Directory.CreateDirectory(bundlePath);

			var filePath = Path.Combine(bundlePath, Prefab.name + ".png");

			WriteImage(filePath, PreviewTexture);
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
