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
				Traverse.Create(__instance).Field<Type[]>("availableTransitions").Value = new Type[10]
				{
					typeof(PlayState),
					typeof(GearSelectionState),
					typeof(TutorialReplayState),
					typeof(TutorialMenuState),
					typeof(FeetControlTutorialState),
					typeof(LevelSelectionState),
					typeof(ChallengeSelectionState),
					typeof(SettingsState),
					typeof(ReplayMenuState),
					typeof(ObjectMovementState)
				};
			}
		}
	}
}
