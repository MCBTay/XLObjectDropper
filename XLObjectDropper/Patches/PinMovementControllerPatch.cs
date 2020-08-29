using GameManagement;
using HarmonyLib;
using UnityEngine;
using XLObjectDropper.Controllers;
using XLObjectDropper.GameManagement;

namespace XLObjectDropper.Patches
{
	public class PinMovementControllerPatch
	{
		[HarmonyPatch(typeof(PinMovementController), "Update")]
		static class UpdatePatch
		{
			static bool Prefix(PinMovementController __instance)
			{
				if (GameStateMachine.Instance.CurrentState.GetType() == typeof(ObjectDropperState))
				{
					var traverseInst = Traverse.Create(__instance);

                    Vector2 axis2D = PlayerController.Instance.inputController.player.GetAxis2D("LeftStickX", "LeftStickY");
                    float axis = PlayerController.Instance.inputController.player.GetAxis(21);
                    float a = (PlayerController.Instance.inputController.player.GetAxis(9) - PlayerController.Instance.inputController.player.GetAxis(8)) * Time.deltaTime * __instance.heightChangeSpeed * __instance.HeightToHeightChangeSpeedCurve.Evaluate(traverseInst.Field("targetHeight").GetValue<float>());
                    traverseInst.Field("currentHeight").SetValue(__instance.transform.position.y - traverseInst.Field("groundLevel").GetValue<float>());
                    traverseInst.Field("currentMoveSpeed").SetValue(Mathf.MoveTowards(traverseInst.Field("currentMoveSpeed").GetValue<float>(), __instance.MoveSpeed * __instance.HeightToMoveSpeedFactorCurve.Evaluate(traverseInst.Field("targetHeight").GetValue<float>()), __instance.HorizontalAcceleration * Time.deltaTime));
                    traverseInst.Field("collisionFlags").SetValue(__instance.characterController.Move(__instance.transform.rotation * new Vector3(axis2D.x, 0.0f, axis2D.y) * traverseInst.Field("currentMoveSpeed").GetValue<float>() * Time.deltaTime));
                    traverseInst.Field("currentHeight").SetValue(__instance.transform.position.y - traverseInst.Field("groundLevel").GetValue<float>());
                    if (!Mathf.Approximately(a, 0.0f))
                    {
                        if ((double)traverseInst.Field("currentHeight").GetValue<float>() < (double)__instance.maxHeight && (double)a > 0.0 || (double)traverseInst.Field("currentHeight").GetValue<float>() > (double)__instance.minHeight && (double)a < 0.0)
	                        traverseInst.Field("collisionFlags").SetValue(__instance.characterController.Move(Vector3.up * a));
                        traverseInst.Field("currentHeight").SetValue(__instance.transform.position.y - traverseInst.Field("groundLevel").GetValue<float>());
                        traverseInst.Field("targetHeight").SetValue(Mathf.Clamp(traverseInst.Field("currentHeight").GetValue<float>(), __instance.minHeight, __instance.maxHeight));
                    }
                    else
                    {
                        float num = (float)(((double)traverseInst.Field("targetHeight").GetValue<float>() - (double)traverseInst.Field("currentHeight").GetValue<float>()) / 0.25);
                        traverseInst.Field("collisionFlags").SetValue(__instance.characterController.Move((Mathf.Approximately(traverseInst.Field("lastVerticalVelocity").GetValue<float>(), 0.0f) || (double)Mathf.Sign(num) == (double)Mathf.Sign(traverseInst.Field("lastVerticalVelocity").GetValue<float>()) ? ((double)Mathf.Abs(num) <= (double)Mathf.Abs(traverseInst.Field("lastVerticalVelocity").GetValue<float>()) ? num : Mathf.MoveTowards(traverseInst.Field("lastVerticalVelocity").GetValue<float>(), num, __instance.VerticalAcceleration * Time.deltaTime)) : 0.0f) * Time.deltaTime * Vector3.up));
                        traverseInst.Field("lastVerticalVelocity").SetValue(__instance.characterController.velocity.y);
                    }

                    traverseInst.Field("currentHeight").SetValue(__instance.transform.position.y - traverseInst.Field("groundLevel").GetValue<float>());
                    __instance.transform.Rotate(0.0f, axis * Time.deltaTime * __instance.RotateSpeed, 0.0f);
                    
                    if (!ObjectMovementController.Instance.LockCameraMovement)
                    {
	                    traverseInst.Method("MoveCamera", false).GetValue();
                    }

                    return false;
				}

				return true;
			}
		}
	}
}
