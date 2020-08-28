using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using XLObjectDropper.UI;
using Object = UnityEngine.Object;

namespace XLObjectDropper
{
	public static class AssetBundleHelper
	{
		public static Dictionary<SpawnableType, List<Spawnable>> LoadedSpawnables { get; private set; }

		static AssetBundleHelper()
		{
			LoadedSpawnables = new Dictionary<SpawnableType, List<Spawnable>>();
		}

		public static void LoadDefaultBundles()
		{
			PlayerController.Instance.StartCoroutine(LoadBundleAsync("XLObjectDropper.Assets.object_testbundle", true));
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
					PlayerController.Instance.StartCoroutine(LoadBundleAsync(assetPack));
				}
				catch (Exception e)
				{
					UnityModManager.Logger.Log("XLObjectDropper: Failed to load asset pack: " + assetPack);
				}
			}
		}

		public static void DisposeLoadedAssets()
		{
			foreach (var type in LoadedSpawnables)
			{
				foreach (var spawnable in type.Value)
				{
					spawnable.Prefab.SetActive(false);
					Object.Destroy(spawnable.Prefab);
				}
			}

			LoadedSpawnables.Clear();
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

			var bundle = abCreateRequest.assetBundle;
			if (bundle == null) yield break;

			var assetLoadRequest = bundle.LoadAllAssetsAsync<GameObject>();
			yield return assetLoadRequest;

			var assets = assetLoadRequest.allAssets;
			if (assets == null || !assets.Any()) yield break;

			foreach (var asset in assets)
			{
				if (!LoadedSpawnables.ContainsKey(isEmbedded ? SpawnableType.Basic : SpawnableType.Packs))
				{
					LoadedSpawnables.Add(isEmbedded ? SpawnableType.Basic : SpawnableType.Packs, new List<Spawnable>());
				}
				LoadedSpawnables[isEmbedded ? SpawnableType.Basic : SpawnableType.Packs].Add(new Spawnable { Prefab = asset as GameObject });
			}

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
