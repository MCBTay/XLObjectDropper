using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using XLObjectDropper.SpawnableScripts;
using XLObjectDropper.UI.Utilities;

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
			AssetPacksPath = Path.Combine(Main.ModPath, "AssetPacks");

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

			foreach (var asset in assets)
			{
				var type = Enumerations.SpawnableType.Rails;

				if (!isEmbedded)
				{
					type = Enumerations.SpawnableType.Packs;
				}
				else
				{
					if (asset.name.StartsWith("grind", StringComparison.InvariantCultureIgnoreCase) || asset.name.StartsWith("coping", StringComparison.InvariantCultureIgnoreCase))
					{
						type = Enumerations.SpawnableType.Splines;
					}
					else
					{
						type = Enumerations.SpawnableType.Rails;
					}
				}

				var gameObject = asset as GameObject;

				var styleGroupController = gameObject.GetComponent<StyleGroupController>();
				var styleController = gameObject.GetComponent<StyleController>();

				if (styleController == null && styleGroupController == null)
				{
					SpawnableManager.Prefabs.Add(new Spawnable(type, gameObject, bundle));
				}
				else if (styleGroupController != null)
				{
					foreach (var styleObject in styleGroupController.Objects)
					{
						var component = styleObject.GetComponent<StyleController>();
						if (component.ShowInObjectSelection)
						{
							var altStyles = styleGroupController.Objects.Where(x => !x.GetComponent<StyleController>().ShowInObjectSelection).ToList();
							SpawnableManager.Prefabs.Add(new Spawnable(type, styleObject, bundle, altStyles));
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
