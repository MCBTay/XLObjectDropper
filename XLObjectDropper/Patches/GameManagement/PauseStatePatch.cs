using GameManagement;
using HarmonyLib;
using System;
using XLObjectDropper.GameManagement;

namespace XLObjectDropper.Patches.GameManagement
{
	public class PauseStatePatch
	{
		[HarmonyPatch(typeof(PauseState), nameof(PauseState.OnEnter))]
		static class OnEnterPatch
		{
			static void Postfix(PauseState __instance)
			{
				var availableTransitions = Traverse.Create(__instance).Field<Type[]>("availableTransitions").Value;
				Array.Resize(ref availableTransitions, availableTransitions.Length + 1);
				availableTransitions[availableTransitions.Length - 1] = typeof(ObjectDropperState);

				Traverse.Create(__instance).Field<Type[]>("availableTransitions").Value = availableTransitions;
			}
		}
	}
}
