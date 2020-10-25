using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using XLObjectDropper.Controllers.ObjectEdit;
using XLObjectDropper.SpawnableScripts;

namespace XLObjectDropper.Utilities
{
	public class Spawnable
	{
		public Enumerations.SpawnableType Type;
		public GameObject Prefab;
		public GameObject SpawnedInstance;
		public Texture2D PreviewTexture;
		public LayerInfo PrefabLayerInfo;
		public string BundleName;
		public string MenuText;

		public List<IObjectSettings> Settings;

		public Spawnable(Enumerations.SpawnableType type, GameObject prefab, string bundleName, string menuText = null, bool generatePreview = true)
		{
			Type = type;
			Prefab = prefab;
			BundleName = bundleName;
			MenuText = menuText;

			PrefabLayerInfo = prefab.transform.GetObjectLayers();

			InitializeSettings();

			if (generatePreview)
			{
				LoadPreviewImage();
			}
		}

		public Spawnable(Enumerations.SpawnableType type, GameObject prefab, string bundleName, string menuText, List<GameObject> alternateStyles) : this(type, prefab, bundleName, menuText)
		{
			var styleController = new EditStyleController();

			foreach (var altStyle in alternateStyles)
			{
				styleController.Styles.Add(new Spawnable(type, altStyle, bundleName));
			}

			foreach (var style in styleController.Styles)
			{
				var newStyleController = new EditStyleController();

				newStyleController.Styles.Add(this);
				newStyleController.Styles.AddRange(styleController.Styles.Where(x => x != style));

				style.Settings.Insert(1, newStyleController);
			}

			Settings.Insert(1, styleController);
		}

		public Spawnable(Spawnable spawnable, GameObject spawnedinstance) : this(spawnable.Type, spawnable.Prefab, spawnable.BundleName, spawnable.MenuText, false)
		{
			SpawnedInstance = spawnedinstance;
			PreviewTexture = spawnable.PreviewTexture;

			if (spawnable.Settings != null && spawnable.Settings.Any())
			{
				var styleController = spawnable.Settings.FirstOrDefault(x => x is EditStyleController);

				if (styleController != null)
				{
					Settings.Insert(1, styleController);
				}
			}
		}

		private void InitializeSettings()
		{
			Settings = new List<IObjectSettings>();

			Settings.Add(new EditGeneralController());

			if (Prefab.GetChildrenOnLayer("Grindable").Any() || Prefab.GetChildrenOnLayer("Coping").Any())
			{
				Settings.Add(new EditGrindablesController { GrindableEnabled = true, CopingEnabled = true });
			}

			if (Prefab.GetComponentsInChildren<Light>(true).Any())
			{
				Settings.Add(new EditLightController());
			}

			if (Prefab.GetComponentInChildren<Animator>(true))
			{
				Settings.Add(new EditAvatarAnimatorController());
			}

			if (Prefab.GetComponentsInChildren<MeshRenderer>(true).Any())
			{
				Settings.Add(new EditColorTintController());
			}
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
