using XLMultiplayerServer;

namespace XLObjectDropper.Multiplayer.Server
{
	public class Main
	{
		private static Plugin pluginInfo;
		

		public static void Load(Plugin info)
		{
			pluginInfo = info;
			pluginInfo.OnToggle = OnToggle;
			pluginInfo.ProcessMessage = ProccessMessage;
		}

		private static void OnToggle(bool enabled)
		{
			if (enabled)
			{
				//string settingsString = File.ReadAllText(Path.Combine(pluginInfo.path, "Settings.json"));

			}
			else
			{
				
			}
		}


		private static void ProccessMessage(PluginPlayer sender, byte[] message)
		{
			// Need to keep up an internal list here, but for now just broadcast.

			foreach (var player in pluginInfo.playerList)
			{
				pluginInfo.SendMessage(pluginInfo, player.GetPlayer(), message, true);
			}
		}
	}
}
