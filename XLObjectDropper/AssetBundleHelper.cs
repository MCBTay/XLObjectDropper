using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XLObjectDropper
{
	public class AssetBundleHelper
	{
		public static List<GameObject> LoadedAssets { get; set; }

		public static void LoadDefaultBundles()
		{
			LoadedAssets = new List<GameObject>();

			LoadBundle("XLObjectDropper.Assets.spawnables_1", true);
			LoadBundle("XLObjectDropper.Assets.spawnables_2", true);
		}

		private static void LoadBundle(string name, bool isEmbedded = false)
		{
			AssetBundle bundle = null;

			if (isEmbedded) bundle = AssetBundle.LoadFromMemory(ExtractResource(name));
			else bundle = AssetBundle.LoadFromFile(name);

			var assets = bundle.LoadAllAssets<GameObject>();
			Debug.Log("Loaded " + assets.Length + " assets from " + name + ".");

			LoadedAssets.AddRange(assets);
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
