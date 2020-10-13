using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using XLObjectDropper.SpawnableScripts;

namespace XLObjectDropper.Utilities
{
	public static class AssetBundleHelper
	{
		public static string ImagesPath;
		public static string AssetPacksPath;

		public static void LoadDefaultBundles()
		{
			ImagesPath = Path.Combine(Main.ModPath, "Images");

			if (!Directory.Exists(ImagesPath))
			{
				Directory.CreateDirectory(ImagesPath);
			}

			PlayerController.Instance.StartCoroutine(LoadBundleAsync("XLObjectDropper.Assets.object_testbundle", true));
			PlayerController.Instance.StartCoroutine(LoadBundleAsync("XLObjectDropper.Assets.sdt - modular", true));
		}

		public static void LoadUserBundles()
		{
			AssetPacksPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "SkaterXL", "XLObjectDropper", "Asset Packs");

			if (!Directory.Exists(AssetPacksPath))
			{
				Directory.CreateDirectory(AssetPacksPath);
			}

			foreach (var assetPack in Directory.GetFiles(AssetPacksPath, "*", SearchOption.AllDirectories))
			{
				if (Path.HasExtension(assetPack)) continue;

				var bundlePath = Path.Combine(ImagesPath, Path.GetFileName(assetPack));
				if (!Directory.Exists(bundlePath))
				{
					Directory.CreateDirectory(bundlePath);
				}

				try
				{
					PlayerController.Instance.StartCoroutine(LoadBundleAsync(assetPack));
				}
				catch (Exception e)
				{
					UnityModManager.Logger.Log("XLObjectDropper: Failed to load asset pack: " + assetPack);
				}
			}
		}

		static IEnumerator LoadBundleAsync(string name, bool isEmbedded = false)
		{
			AssetBundleCreateRequest abCreateRequest;

			if (isEmbedded)
			{
				abCreateRequest = AssetBundle.LoadFromMemoryAsync(ExtractResource(name));
			}
			else
			{
				abCreateRequest = AssetBundle.LoadFromFileAsync(name);
			}

			yield return abCreateRequest;

			var bundle = abCreateRequest?.assetBundle;
			if (bundle == null) yield break;

			var assetLoadRequest = bundle.LoadAllAssetsAsync<GameObject>();
			yield return assetLoadRequest;

			var assets = assetLoadRequest.allAssets;
			if (assets == null || !assets.Any()) yield break;

			foreach (GameObject asset in assets)
			{
				var type = Enumerations.SpawnableType.Other;
				var menuText = string.Empty;

				var categoryController = asset.GetComponentInChildren<XLCategoryController>(true);
				if (categoryController != null)
				{
					type = categoryController.Type;
					menuText = categoryController.MenuText;
				}

				var styleGroupController = asset.GetComponent<XLStyleGroupController>();
				var styleController = asset.GetComponent<XLStyleController>();

				if (styleController == null && styleGroupController == null)
				{
					SpawnableManager.Prefabs.Add(new Spawnable(type, asset, bundle.name, menuText));
				}
				else if (styleGroupController != null)
				{
					foreach (var styleObject in styleGroupController.Objects)
					{
						var component = styleObject.GetComponent<XLStyleController>();
						if (component.ShowInObjectSelection)
						{
							var styleCatController = styleObject.GetComponentInChildren<XLCategoryController>(true);
							if (styleCatController != null)
							{
								type = styleCatController.Type;
								menuText = styleCatController.MenuText;
							}

							var altStyles = styleGroupController.Objects.Where(x => !x.GetComponent<XLStyleController>().ShowInObjectSelection).ToList();
							SpawnableManager.Prefabs.Add(new Spawnable(type, styleObject, bundle.name, menuText, altStyles));
						}
					}
				}
			}

			SelfieCamera.enabled = false;

			bundle.Unload(false);
		}

		public static GameObject UIPrefab { get; set; }
		public static GameObject CustomPassPrefab { get; set; }
		public static Camera SelfieCamera { get; set; }
		public static GameObject GridOverlayPrefab { get; set; }
		public static GameObject UnsavedChangesDialogPrefab { get; set; }

		public static void LoadUIBundle()
		{
			AssetBundle bundle = AssetBundle.LoadFromMemory(ExtractResource("XLObjectDropper.Assets.ui_bundle"));

			var prefabPath = "Assets/OBJ_Dropper_Bundles/UI_Bundle/";

			UIPrefab = bundle.LoadAsset<GameObject>(prefabPath + "ObjDrop_UI.prefab");
			CustomPassPrefab = bundle.LoadAsset<GameObject>(prefabPath + "Outline Custom Pass.prefab");
			UnsavedChangesDialogPrefab = bundle.LoadAsset<GameObject>(prefabPath + "UnsavedChangesDialog_Master.prefab");

			GridOverlayPrefab = bundle.LoadAsset<GameObject>("Assets/GridLayout/Grid_Overlay.prefab");

			SelfieCamera = bundle.LoadAsset<GameObject>(prefabPath + "SelfieCamera.prefab").GetComponent<Camera>();

			bundle.Unload(false);
		}

		private static byte[] ExtractResource(string filename)
		{
			Assembly a = Assembly.GetExecutingAssembly();
			using (var resFilestream = a.GetManifestResourceStream(filename))
			{
				if (resFilestream == null) return null;
				byte[] ba = new byte[resFilestream.Length];
				resFilestream.Read(ba, 0, ba.Length);
				return ba;
			}
		}
	}
}
