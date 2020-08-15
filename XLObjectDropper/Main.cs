using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using XLObjectDropper.UserInterface;

namespace XLObjectDropper
{
	static class Main
	{
		public static bool Enabled { get; private set; }
		private static Harmony Harmony { get; set; }

		static bool Load(UnityModManager.ModEntry modEntry)
		{
			modEntry.OnToggle = OnToggle;

			return true;
		}

		public static GameObject ObjectMovementController;

		private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
		{
			if (Enabled == value) return true;
			Enabled = value;

			if (Enabled)
			{
				Harmony = new Harmony(modEntry.Info.Id);
				Harmony.PatchAll(Assembly.GetExecutingAssembly());

				UserInterfaceHelper.CreateObjectDropperButton();
				AssetBundleHelper.LoadDefaultBundles();

				ObjectMovementController = new GameObject();
				ObjectMovementController.AddComponent<ObjectMovementController>();
				Object.DontDestroyOnLoad(ObjectMovementController);
			}
			else
			{
				Harmony.UnpatchAll(Harmony.Id);
			}

			return true;
		}

		
	}
}
