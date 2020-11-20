using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XLMultiplayer;
using XLObjectDropper.Controllers;
using XLObjectDropper.Multiplayer.Client;
using XLObjectDropper.Multiplayer.Client.Patches;
using XLObjectDropper.Utilities;
using XLObjectDropper.Utilities.Save;

namespace Test
{
	public class Main
	{
		public static Plugin pluginInfo;

		private static void Load(Plugin plugin)
		{
			pluginInfo = plugin;
			pluginInfo.OnToggle = OnToggle;
			pluginInfo.ProcessMessage = ProcessMessage;
		}

		private static void OnToggle(bool enabled)
		{
			if (enabled)
			{
				
				if (AccessTools.TypeByName(nameof(XLObjectDropper.Settings)) != null)
				{
					SpawnableManager.DeleteSpawnedObjects();
				}
				else
				{
					HandleFailure();
				}
			}
		}

		//1st byte, what we want to do
		//check out opcode in MultiplayerController


		// this means we received a message from the server
		private static void ProcessMessage(byte[] message)
		{
			if (message.Length > 1)
			{
				var gameObjectData = message.Skip(1).ToArray();
				
				var deserialized = gameObjectData.DeserializeFromBytes<GameObjectSaveData>();

				switch (message[0])
				{
					case (byte)ObjectMovementControllerPatch.OpCode.ObjectCreated:
						deserialized.Instantiate();
						break;
					case (byte)ObjectMovementControllerPatch.OpCode.ObjectMoved:
						break;
					case (byte)ObjectMovementControllerPatch.OpCode.ObjectDeleted:
						break;
				}
			}
		}

		//private static XXLMod.Settings GetSettings()
		//{
		//	return Traverse.Create(AccessTools.TypeByName("XXLMod.Main")).Field("settings").GetValue<XXLMod.Settings>();
		//}

		//private static void SetSettings(XXLMod.Settings settings)
		//{
		//	Traverse.Create(AccessTools.TypeByName("XXLMod.Main")).Field("settings").SetValue(settings);
		//}

		private static void HandleFailure()
		{
			pluginInfo.SendMessage(pluginInfo, new byte[] { 0 }, true);
		}

		private static async void CheckStats()
		{
			while (pluginInfo.enabled)
			{
				// TODO: Get settings here and send to server

				//XXLMod.Settings stats = GetSettings();

				List<byte> sendMessage = new List<byte>();
				//sendMessage.AddRange(BitConverter.GetBytes(stats.gravity));
				//sendMessage.AddRange(BitConverter.GetBytes(stats.popForce));
				//sendMessage.AddRange(BitConverter.GetBytes(stats.highPopForce));
				//sendMessage.AddRange(BitConverter.GetBytes(stats.grindPopForce));
				//sendMessage.AddRange(BitConverter.GetBytes(stats.manualPopForce));
				//sendMessage.AddRange(BitConverter.GetBytes(stats.flipSpeed));
				//sendMessage.AddRange(BitConverter.GetBytes(stats.scoopSpeed));
				//sendMessage.AddRange(BitConverter.GetBytes(stats.pushForce));
				//sendMessage.AddRange(BitConverter.GetBytes(stats.topSpeed));
				//sendMessage.AddRange(BitConverter.GetBytes(stats.bodySpin));
				//sendMessage.AddRange(BitConverter.GetBytes(stats.maxPumpSpeed));
				//sendMessage.AddRange(BitConverter.GetBytes(stats.manualCatch));

				pluginInfo.SendMessage(pluginInfo, sendMessage.ToArray(), true);

				await Task.Delay(10000);
			}
		}
	}
}
