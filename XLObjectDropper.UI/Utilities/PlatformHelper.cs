using System.Linq;
using UnityEngine;

namespace XLObjectDropper.UI.Utilities
{
	public class PlatformHelper
	{
		public static PlatformType GetPlatformType()
		{
  			var player = UIManager.Instance.Player;

			if (player == null)
			{
				return PlatformType.Xbox;
			}

			switch (Application.platform)
			{
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsEditor:
					string str = player.controllers.Joysticks.FirstOrDefault()?.name ?? "unknown";
					if (str.Contains("Dual Shock") || str.Contains("DualShock"))
					{
						return PlatformType.Playstation;
					}

					return PlatformType.Xbox;
				case RuntimePlatform.PS4:
					return PlatformType.Playstation;
					break;
				case RuntimePlatform.Switch:
					return PlatformType.Switch;
				case RuntimePlatform.XboxOne:
				default:
					return PlatformType.Xbox;
			}
		}
	}
}
