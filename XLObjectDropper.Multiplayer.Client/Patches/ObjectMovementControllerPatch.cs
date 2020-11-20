using HarmonyLib;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Test;
using UnityEngine;
using UnityEngine.Playables;
using XLMultiplayer;
using XLObjectDropper.Controllers;
using XLObjectDropper.GameManagement;
using XLObjectDropper.Utilities;
using XLObjectDropper.Utilities.Save;

namespace XLObjectDropper.Multiplayer.Client.Patches
{
	public class ObjectMovementControllerPatch
	{
		public enum OpCode : byte
		{
			ObjectCreated = 0,
			ObjectMoved = 1,
			ObjectEdited = 2,
			ObjectDeleted = 3,
			ObjectRotated = 4,
			ObjectScaled = 5
		}

		[HarmonyPatch(typeof(ObjectMovementController), "PlaceObject")]
		static class PlaceObjectPatch
		{
			static void Prefix(ObjectMovementController __instance, ref bool ___existingObject)
			{
				byte[] payload = new byte[] {};
				payload.AddToArray(___existingObject ? (byte)OpCode.ObjectMoved : (byte)OpCode.ObjectCreated);

				var spawnable = __instance.SelectedObject.GetSpawnable();

				var objectSaveData = new GameObjectSaveData
				{
					Id = spawnable.Prefab.name,
					bundleName = spawnable.BundleName,
					position = new SerializableVector3(__instance.SelectedObject.transform.position),
					rotation = new SerializableQuaternion(__instance.SelectedObject.transform.rotation),
					localScale = new SerializableVector3(__instance.SelectedObject.transform.localScale)
				};

				foreach (var settings in spawnable.Settings)
				{
					var settingsSaveData = settings.ConvertToSaveSettings();
					if (settingsSaveData != null)
					{
						objectSaveData.settings.Add(settingsSaveData);
					}
				}

				payload.AddRangeToArray(objectSaveData.SerializeToBytes());
				Main.pluginInfo.SendMessage(Main.pluginInfo, payload, true);
			}
		}


		[HarmonyPatch(typeof(ObjectMovementController), "DeleteObject")]
		static class DeleteObjectPatch
		{
			static void Prefix(ObjectMovementController __instance)
			{
				byte[] payload = new byte[] { };
				payload.AddToArray((byte)OpCode.ObjectDeleted);

				var spawnable = __instance.HighlightedObject.GetSpawnableFromSpawned();

				var objectSaveData = new GameObjectSaveData
				{
					Id = spawnable.Prefab.name,
					bundleName = spawnable.BundleName,
					position = new SerializableVector3(__instance.SelectedObject.transform.position),
					rotation = new SerializableQuaternion(__instance.SelectedObject.transform.rotation),
					localScale = new SerializableVector3(__instance.SelectedObject.transform.localScale)
				};

				foreach (var settings in spawnable.Settings)
				{
					var settingsSaveData = settings.ConvertToSaveSettings();
					if (settingsSaveData != null)
					{
						objectSaveData.settings.Add(settingsSaveData);
					}
				}

				payload.AddRangeToArray(objectSaveData.SerializeToBytes());
				Main.pluginInfo.SendMessage(Main.pluginInfo, payload, true);
			}
		}
	}
}
