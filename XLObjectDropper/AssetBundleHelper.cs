using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using Object = UnityEngine.Object;

namespace XLObjectDropper
{
	public static class AssetBundleHelper
	{
		public static List<GameObject> LoadedAssets { get; set; }

		static AssetBundleHelper()
		{
			LoadedAssets = new List<GameObject>();
		}

		public static void LoadDefaultBundles()
		{
			LoadBundle("XLObjectDropper.Assets.object_testbundle", true);
		}

		public static void LoadUserBundles()
		{
			var assetPackDirectory = Path.Combine(Main.ModPath, "AssetPacks");

			if (!Directory.Exists(assetPackDirectory))
			{
				Directory.CreateDirectory(assetPackDirectory);
			}

			foreach (var assetPack in Directory.GetFiles(assetPackDirectory, "*", SearchOption.AllDirectories))
			{
				if (Path.HasExtension(assetPack)) continue;

				try
				{
					LoadBundle(assetPack);
				}
				catch (Exception e)
				{
					UnityModManager.Logger.Log("XLObjectDropper: Failed to load asset pack: " + assetPack);
				}
			}
		}

		public static void DisposeLoadedAssets()
		{
			foreach (var gameObject in LoadedAssets)
			{
				gameObject.SetActive(false);
				Object.Destroy(gameObject);
			}

			LoadedAssets.Clear();
		}

		private static void LoadBundle(string name, bool isEmbedded = false)
		{
			AssetBundle bundle = null;

			if (isEmbedded) bundle = AssetBundle.LoadFromMemory(ExtractResource(name));
			else bundle = AssetBundle.LoadFromFile(name);

			var assets = bundle.LoadAllAssets<GameObject>();
			Debug.Log("Loaded " + assets.Length + " assets from " + name + ".");

			LoadedAssets.AddRange(assets);

			bundle.Unload(false);
		}

		public static GameObject UIPrefab { get; set; }
		public static GameObject ListItemPrefab { get; set; }

		public static void LoadUIBundle()
		{
			AssetBundle bundle = AssetBundle.LoadFromMemory(ExtractResource("XLObjectDropper.Assets.ui_bundle"));

			UIPrefab = bundle.LoadAsset<GameObject>("Assets/OBJ_Dropper_Bundles/UI_Bundle/ObjDrop_UI.prefab");
			ListItemPrefab = bundle.LoadAsset<GameObject>("Assets/OBJ_Dropper_Bundles/UI_Bundle/ListItem.prefab");

			bundle.Unload(false);
		}

		//private static IEnumerator LoadBundle(string name, bool isEmbedded = false)
		//{
		//	AssetBundleCreateRequest bundleLoadRequest = null;

		//	if (isEmbedded)
		//	{
		//		bundleLoadRequest = AssetBundle.LoadFromMemoryAsync(ExtractResource(name));
		//	}
		//	else
		//	{
		//		bundleLoadRequest = AssetBundle.LoadFromFileAsync(name);
		//	}
		//	yield return bundleLoadRequest;

		//	var myLoadedAssetBundle = bundleLoadRequest.assetBundle;
		//	if (myLoadedAssetBundle == null)
		//	{
		//		Debug.Log("Failed to load AssetBundle: " + name);
		//		yield break;
		//	}

		//	var assetLoadRequest = myLoadedAssetBundle.LoadAllAssetsAsync();
		//	yield return assetLoadRequest;

		//	LoadedAssets.AddRange(assetLoadRequest.allAssets);

		//	myLoadedAssetBundle.Unload(false);
		//}

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
