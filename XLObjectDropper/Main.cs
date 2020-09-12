using HarmonyLib;
using System.Reflection;
using UnityModManagerNet;
using XLObjectDropper.UserInterface;
using XLObjectDropper.Utilities;

namespace XLObjectDropper
{
#if DEBUG
	[EnableReloading]
#endif
	static class Main
	{
		public static bool Enabled { get; private set; }
		private static Harmony Harmony { get; set; }
		public static string ModPath { get; set; }

		static bool Load(UnityModManager.ModEntry modEntry)
		{
			Settings.Instance = UnityModManager.ModSettings.Load<Settings>(modEntry);

			ModPath = modEntry.Path;
			Utilities.SaveManager.Instance.ModEntry = modEntry;

			modEntry.OnToggle = OnToggle;
#if DEBUG
			modEntry.OnUnload = Unload;
#endif

			return true;
		}

		private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
		{
			if (Enabled == value) return true;
			Enabled = value;

			if (Enabled)
			{
				Harmony = new Harmony(modEntry.Info.Id);
				Harmony.PatchAll(Assembly.GetExecutingAssembly());

				UserInterfaceHelper.CreateObjectDropperButton();
				UserInterfaceHelper.LoadUserInterface();
				AssetBundleHelper.LoadDefaultBundles();
				AssetBundleHelper.LoadUserBundles();
			}
			else
			{
				Harmony.UnpatchAll(Harmony.Id);
			}

			return true;
		}

#if DEBUG
		static bool Unload(UnityModManager.ModEntry modEntry)
		{
			UserInterfaceHelper.DestroyObjectDropperButton();
			AssetBundleHelper.DisposeLoadedAssets();

			Harmony?.UnpatchAll();
			return true;
		}
#endif
	}
}
