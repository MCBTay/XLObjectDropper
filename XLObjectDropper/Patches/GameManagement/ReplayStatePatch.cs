using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameManagement;
using HarmonyLib;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Patches.GameManagement
{
	public class ReplayStatePatch
	{
		[HarmonyPatch(typeof(ReplayState), nameof(ReplayState.OnEnter))]
		static class OnEnterPatch
		{
			static void Postfix(ReplayState __instance)
			{
				if (SpawnableManager.SpawnedObjects != null &&
				    SpawnableManager.SpawnedObjects.Any(x => x.HideInReplays))
				{
					foreach (var spawnable in SpawnableManager.SpawnedObjects.Where(x => x.HideInReplays))
					{
						spawnable.SpawnedInstance.SetActive(false);
					}
				}
			}
		}

		[HarmonyPatch(typeof(ReplayState), nameof(ReplayState.OnExit))]
		static class OnExitPatch
		{
			static void Postfix(ReplayState __instance)
			{
				if (SpawnableManager.SpawnedObjects != null &&
				    SpawnableManager.SpawnedObjects.Any(x => x.HideInReplays))
				{
					foreach (var spawnable in SpawnableManager.SpawnedObjects.Where(x => x.HideInReplays))
					{
						spawnable.SpawnedInstance.SetActive(true);
					}
				}
			}
		}
	}
}
