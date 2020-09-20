using GameManagement;
using HarmonyLib;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Patches
{
	public class LevelManagerPatch
	{
		[HarmonyPatch(typeof(LevelManager), nameof(LevelManager.PlayLevel))]
		static class PlayLevelPatch
		{
			static void Prefix(LevelManager __instance, LevelInfo level)
			{
				if (level.Equals(LevelManager.Instance.currentLevel) || GameStateMachine.Instance.IsLoading)
					return;

				// Display Dialog here
				// If yes, save out objects
				//var test = UnityEngine.Object.Instantiate(original: AssetBundleHelper.UnsavedChangesDialogPrefab);
				//test.SetActive(true);


				//Utilities.SaveManager.Instance.SaveCurrentSpawnables();

				SpawnableManager.DeleteSpawnedObjects();

				return;
			}
		}
	}
}
