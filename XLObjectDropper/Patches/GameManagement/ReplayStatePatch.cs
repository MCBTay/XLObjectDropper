using GameManagement;
using HarmonyLib;
using System.Linq;
using XLObjectDropper.Controllers.ObjectEdit;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Patches.GameManagement
{
	public class ReplayStatePatch
	{
		[HarmonyPatch(typeof(ReplayState), nameof(ReplayState.OnEnter))]
		static class OnEnterPatch
		{
			static void Postfix()
			{
				if (SpawnableManager.SpawnedObjects != null && SpawnableManager.SpawnedObjects.Any())
				{
					foreach (var spawnable in SpawnableManager.SpawnedObjects)
					{
						if (spawnable.Settings != null && spawnable.Settings.FirstOrDefault(x => x is EditGeneralController) is EditGeneralController generalSettings && generalSettings.HideInReplays)
						{
							spawnable.SpawnedInstance.SetActive(false);
						}
					}
				}
			}
		}

		[HarmonyPatch(typeof(ReplayState), nameof(ReplayState.OnExit))]
		static class OnExitPatch
		{
			static void Postfix()
			{
				if (SpawnableManager.SpawnedObjects != null && SpawnableManager.SpawnedObjects.Any())
				{
					foreach (var spawnable in SpawnableManager.SpawnedObjects)
					{
						if (spawnable.Settings != null && spawnable.Settings.FirstOrDefault(x => x is EditGeneralController) is EditGeneralController generalSettings && generalSettings.HideInReplays)
						{
							spawnable.SpawnedInstance.SetActive(true);
						}
					}
				}
			}
		}
	}
}
