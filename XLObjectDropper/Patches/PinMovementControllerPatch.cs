using System;
using GameManagement;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using XLObjectDropper.Controllers;
using XLObjectDropper.GameManagement;

namespace XLObjectDropper.Patches
{
	public static class PinMovementControllerPatch
	{
		private static Vector3 initialVector = Vector3.forward;
		private static float rotationAngleX;
		private static float rotationAngleY;
		private static float distance;

		[HarmonyPatch(typeof(PinMovementController), "OnEnable")]
		static class OnEnablePatch
		{
			private static void Prefix(PinMovementController __instance)
			{
				if (GameStateMachine.Instance.CurrentState.GetType() == typeof(ObjectDropperState))
				{
					__instance.defaultHeight = 2.5f;
					Traverse.Create(__instance).Field("minHeight").SetValue(0f);
				}
				else
				{
					__instance.defaultHeight = 1.8f;
				}
			}

			private static void Postfix(PinMovementController __instance, ref float ___currentCameraDist)
			{
				if (GameStateMachine.Instance.CurrentState.GetType() == typeof(ObjectDropperState))
				{
					initialVector = __instance.cameraPivot.position - __instance.transform.position;

					rotationAngleX = __instance.cameraPivot.eulerAngles.x;
					rotationAngleY = __instance.cameraPivot.eulerAngles.y;

					distance = ___currentCameraDist;
				}
			}
		}

		[HarmonyPatch(typeof(PinMovementController), "Update")]
		static class UpdatePatch
		{
			private static Transform marker;
			

			static bool Prefix(PinMovementController __instance, ref float ___currentHeight, ref float ___targetHeight, ref float ___currentMoveSpeed, ref float ___groundLevel,
				               ref CollisionFlags ___collisionFlags, ref float ___lastVerticalVelocity, Camera ___mainCam, ref float ___currentCameraDist)
			{
				if (GameStateMachine.Instance.CurrentState.GetType() == typeof(ObjectDropperState))
				{
					Vector2 leftStick = PlayerController.Instance.inputController.player.GetAxis2D("LeftStickX", "LeftStickY");
					Vector2 rightStick = PlayerController.Instance.inputController.player.GetAxis2D("RightStickX", "RightStickY");
					float a = (PlayerController.Instance.inputController.player.GetAxis(9) - PlayerController.Instance.inputController.player.GetAxis(8)) * Time.deltaTime * __instance.heightChangeSpeed * __instance.HeightToHeightChangeSpeedCurve.Evaluate(___targetHeight);

                    ___currentHeight = __instance.transform.position.y - ___groundLevel;
					___currentMoveSpeed = Mathf.MoveTowards(___currentMoveSpeed, __instance.MoveSpeed * __instance.HeightToMoveSpeedFactorCurve.Evaluate(___targetHeight), __instance.HorizontalAcceleration * Time.deltaTime);
                    ___collisionFlags = __instance.characterController.Move(__instance.cameraPivot.transform.rotation * new Vector3(leftStick.x, 0.0f, leftStick.y) * ___currentMoveSpeed * Time.deltaTime);
					___currentHeight = __instance.transform.position.y - ___groundLevel;
					if (!Mathf.Approximately(a, 0.0f))
                    {
						if ((double) ___currentHeight < (double) __instance.maxHeight && (double) a > 0.0 ||
							(double) ___currentHeight > (double) __instance.minHeight && (double) a < 0.0)
						{
							___collisionFlags = __instance.characterController.Move(Vector3.up * a);
						}

						___currentHeight = __instance.transform.position.y - ___groundLevel;
						___targetHeight = Mathf.Clamp(___currentHeight, __instance.minHeight, __instance.maxHeight);
                    }
                    else
                    {
                        float num = (float)(((double)___targetHeight - (double)___currentHeight) / 0.25);
                        ___collisionFlags = __instance.characterController.Move((Mathf.Approximately(___lastVerticalVelocity, 0.0f) || (double)Mathf.Sign(num) == (double)Mathf.Sign(___lastVerticalVelocity) ? ((double)Mathf.Abs(num) <= (double)Mathf.Abs(___lastVerticalVelocity) ? num : Mathf.MoveTowards(___lastVerticalVelocity, num, __instance.VerticalAcceleration * Time.deltaTime)) : 0.0f) * Time.deltaTime * Vector3.up);
                        ___lastVerticalVelocity = __instance.characterController.velocity.y;
                    }

					___currentHeight = __instance.transform.position.y - ___groundLevel;

					#region Camera rotation
					rotationAngleX += rightStick.x * Time.deltaTime * __instance.RotateSpeed;
					rotationAngleY += rightStick.y * Time.deltaTime * __instance.RotateSpeed;

					var maxAngle = 85f;

					rotationAngleY = ClampAngle(rotationAngleY, -maxAngle, maxAngle);

					var toRotation = Quaternion.Euler(rotationAngleY, rotationAngleX, 0);
					var rotation = toRotation;

					Vector3 negDistance = new Vector3(0, 0, -___currentCameraDist);
					var position = rotation * negDistance + Vector3.zero;

					__instance.cameraPivot.rotation = rotation;
					__instance.cameraNode.position = position;
					#endregion

					if (!ObjectMovementController.Instance.LockCameraMovement)
                    {
	                    Traverse.Create(__instance).Method("MoveCamera", false).GetValue();
                    }

                    return false;
				}

				return true;
			}

			private static float ClampAngle(float angle, float min, float max)
			{
				if (angle < -360F)
					angle += 360F;
				if (angle > 360F)
					angle -= 360F;
				return Mathf.Clamp(angle, min, max);
			}
		}

		[HarmonyPatch(typeof(PinMovementController), "MoveCamera", new [] { typeof(bool) })]
		static class MoveCameraPatch
		{
			static bool Prefix(PinMovementController __instance, ref bool moveInstant, ref float ___targetHeight, ref float ___lastCameraVelocity, ref float ___currentCameraDist)
			{
				if (GameStateMachine.Instance.CurrentState.GetType() == typeof(ObjectDropperState))
				{
					Ray ray = new Ray(__instance.cameraPivot.position, -__instance.cameraPivot.forward);
					float num1 = __instance.HeightToCameraDistCurve.Evaluate(___targetHeight);
					
					RaycastHit hitInfo;
					if (Physics.SphereCast(ray, __instance.CameraSphereCastRadius, out hitInfo, num1, (int)__instance.layermask) && (double)(num1 = Mathf.Max(0.02f, hitInfo.distance - __instance.CameraSphereCastRadius)) < (double)___currentCameraDist)
						moveInstant = true;
					
					if (moveInstant)
					{
						___lastCameraVelocity = 0.0f;
						___currentCameraDist = num1;
					}
					else
					{
						float num2 = (float)(((double)___targetHeight - (double)___currentCameraDist) / 0.25);

						float f = Mathf.Approximately(___lastCameraVelocity, 0.0f) || (double)Mathf.Sign(num2) == (double)Mathf.Sign(___lastCameraVelocity) ?
							((double)Mathf.Abs(num2) <= (double)Mathf.Abs(___lastCameraVelocity) ? num2 : Mathf.MoveTowards(___lastCameraVelocity, num2, __instance.MaxCameraAcceleration * Time.deltaTime)) :
							0.0f;
						___currentCameraDist = Mathf.MoveTowards(___currentCameraDist, ___targetHeight, Mathf.Abs(f) * Time.deltaTime);
						___lastCameraVelocity = f;

					}
					__instance.cameraNode.localPosition = new Vector3(0.0f, 0.0f, -___currentCameraDist);

					PlayerController.Instance.cameraController.MoveCameraTo(__instance.cameraNode.position, __instance.cameraNode.rotation);

					return false;
				}

				return true;
			}
		}

		[HarmonyPatch(typeof(PinMovementController), "OnDisable")]
		static class OnDisablePatch
		{
			static void Prefix(PinMovementController __instance)
			{
				if (GameStateMachine.Instance.CurrentState.GetType() == typeof(ObjectDropperState))
				{
					//Vector2 rightStick = PlayerController.Instance.inputController.player.GetAxis2D("RightStickX", "RightStickY");
					__instance.transform.rotation = Quaternion.identity;
					
				}
			}
		}
	}
}
