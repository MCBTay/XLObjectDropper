using HarmonyLib;
using System.Linq;
using UnityEngine;
using XLObjectDropper.Controllers.ObjectEdit;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Patches
{
	public class RespawnPatch
	{
		[HarmonyPatch(typeof(Respawn), "GetSpawnPos")]
		public static class GetSpawnPosPatch
		{
			static void Postfix(bool tutorial)
			{
				if (tutorial) return;

				var spawnablesWithRBs = SpawnableManager.SpawnedObjects.Where(x => x.SpawnedInstance.GetComponentsInChildren<Rigidbody>(true).Any());

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
