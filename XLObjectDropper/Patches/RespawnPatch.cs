using System.Collections.Generic;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using XLObjectDropper.Controllers.ObjectEdit;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Patches
{
	public class RespawnPatch
	{
		[HarmonyPatch(typeof(Respawn), "Start")]
		public static class StartPatch
		{
			static void Postfix(Respawn __instance)
			{
				__instance.OnRespawn += HandleRespawn;
			}

			private static void HandleRespawn()
			{
				var spawnablesWithRBs = new List<Spawnable>();
				foreach (var spawnable in SpawnableManager.SpawnedObjects)
				{
					if (spawnable.SpawnedInstance == null) continue;

					var rigidbodies = spawnable.SpawnedInstance.GetComponentsInChildren<Rigidbody>(true);

					if (rigidbodies != null && rigidbodies.Any())
					{
						spawnablesWithRBs.Add(spawnable);
					}
				}

				foreach (var spawnable in spawnablesWithRBs)
				{
					var rigidbodyController = spawnable.Settings.FirstOrDefault(x => x != null && x is EditRigidbodiesController) as EditRigidbodiesController;
					if (rigidbodyController == null) continue;

					var rigidBodies = spawnable.SpawnedInstance.GetComponentsInChildren<Rigidbody>(true);
					if (rigidBodies == null || !rigidBodies.Any()) continue;

					if (!rigidbodyController.EnableRespawnRecall || rigidbodyController.RecallPosition == Vector3.zero) continue;
					spawnable.SpawnedInstance.transform.position = rigidbodyController.RecallPosition;
					spawnable.SpawnedInstance.transform.rotation = rigidbodyController.RecallRotation;

					foreach (var rigidbody in rigidBodies)
					{
						rigidbody.velocity = Vector3.zero;
						rigidbody.angularVelocity = Vector3.zero;
					}
				}
			}
		}
	}
}
