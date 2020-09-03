using HarmonyLib;

namespace XLObjectDropper.Patches
{
	/// <summary>
	/// This is only here for right now if I need to pull values out of the PinMovementController.  Will get deleted soon.
	/// </summary>
	[HarmonyPatch(typeof(PinMovementController), "OnEnable")]
	static class OnEnablePatch
	{
		private static void Postfix(PinMovementController __instance)
		{
			int x = 5;
		}
	}
}