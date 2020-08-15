using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace XLObjectDropper
{
	public class AssetBundleHelper
	{
		public static List<GameObject> LoadedAssets { get; set; }

		public static void LoadDefaultBundles()
		{
			LoadedAssets = new List<GameObject>();

			LoadBundle("XLObjectDropper.Assets.object_testbundle", true);
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

		public static GameObject LoadUIBundle()
		{
			AssetBundle bundle = AssetBundle.LoadFromMemory(ExtractResource("XLObjectDropper.Assets.ui_bundle"));
			
			GameObject newMenuObject = GameObject.Instantiate(bundle.LoadAsset<GameObject>("ObjDrop_UI"));
			GameObject.DontDestroyOnLoad(newMenuObject);

			return newMenuObject;
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
