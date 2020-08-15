using Rewired;
using System.Linq;
using UnityEngine;

namespace XLObjectDropper.UserInterface
{
	/// <summary>
	/// Maps to the TMP_SpriteAsset ControllerIcons found within the game.
	/// </summary>
	public enum ControllerIconSprite
	{
		SWITCH_L = 0,
		XB1_LB = 1,
		SWITCH_R = 2,
		SWITCH_ZL = 3,
		XB1_RB = 4,
		PS4_TouchPad = 5,
		PS4_L1 = 6,
		SWITCH_DOWN = 7,
		SWITCH_LEFT = 8,
		SWITCH_RIGHT = 9,
		SWITCH_UP = 10,
		SWITCH_ZR = 11,
		XB1_B = 12,
		SWITCH_SL = 13,
		XB1_RT = 14,
		PS4_Circle_Button = 15,
		PS4_Triangle_Button = 16,
		SWITCH_A = 17,
		SWITCH_B = 18,
		PS4_L2 = 19,
		XB1_A = 20,
		PS4_R1 = 21,
		SWITCH_SR = 22,
		XB1_Menu = 23,
		PS4_Cross = 24,
		XB1_X = 25,
		SWITCH_Y = 26,
		PS4_R2 = 27,
		PS4_Square = 28,
		PS4_Circle = 29,
		XB1_XboxButton = 30,
		PS4_Options = 31,
		d_pad_down = 32,
		XB1_LT = 33,
		PS4_LeftStick = 34,
		d_pad_left = 35,
		XB1_View = 36,
		PS4_Square_Button = 37,
		PS4_Cross_Button = 38,
		XB1_Y = 39,
		SWITCH_PLUS = 40,
		SWITCH_MINUS = 41,
		SWITCH_X = 42,
		d_pad_right = 43,
		d_pad_up = 44,
		PS4_Triangle = 45,
		PS4_RightStick = 46,
		XB1_LeftStick = 47
	}

    public class ControllerButtonHelper
	{
		public static int GetSpriteIndex(string requestedButton)
		{
			switch (requestedButton)
			{
				case "X": return GetSpriteIndex_XButton();
				case "Y": return GetSpriteIndex_YButton();
				case "B": return GetSpriteIndex_BButton();
				case "A":
				default:
					return GetSpriteIndex_AButton();
			}
		}

		public static int GetSpriteIndex_AButton()
		{
			ControllerIconSprite returnVal;

			switch (Application.platform)
			{
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsEditor:
					string str = PlayerController.Instance.inputController.player.controllers.Joysticks.FirstOrDefault<Joystick>()?.name ?? "unknown";
					if (str.Contains("Dual Shock") || str.Contains("DualShock"))
					{
						returnVal = ControllerIconSprite.PS4_Cross_Button;
						break;
					}
					returnVal = ControllerIconSprite.XB1_A;
					break;
				case RuntimePlatform.PS4:
					returnVal = ControllerIconSprite.PS4_Cross_Button;
					break;
				case RuntimePlatform.Switch:
					returnVal = ControllerIconSprite.SWITCH_A;
					break;
				case RuntimePlatform.XboxOne:
				default:
					returnVal = ControllerIconSprite.XB1_A;
					break;
			}

			return (int)returnVal;
		}

		public static int GetSpriteIndex_BButton()
		{
			ControllerIconSprite returnVal;

			switch (Application.platform)
			{
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsEditor:
					string str = PlayerController.Instance.inputController.player.controllers.Joysticks.FirstOrDefault<Joystick>()?.name ?? "unknown";
					if (str.Contains("Dual Shock") || str.Contains("DualShock"))
					{
						returnVal = ControllerIconSprite.PS4_Circle_Button;
						break;
					}
					returnVal = ControllerIconSprite.XB1_B;
					break;
				case RuntimePlatform.PS4:
					returnVal = ControllerIconSprite.PS4_Circle_Button;
					break;
				case RuntimePlatform.Switch:
					returnVal = ControllerIconSprite.SWITCH_B;
					break;
				case RuntimePlatform.XboxOne:
				default:
					returnVal = ControllerIconSprite.XB1_B;
					break;
			}

			return (int)returnVal;
		}

		public static int GetSpriteIndex_XButton()
		{
			ControllerIconSprite returnVal;

			switch (Application.platform)
			{
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsEditor:
					string str = PlayerController.Instance.inputController.player.controllers.Joysticks.FirstOrDefault<Joystick>()?.name ?? "unknown";
					if (str.Contains("Dual Shock") || str.Contains("DualShock"))
					{
						returnVal = ControllerIconSprite.PS4_Square_Button;
						break;
					}
					returnVal = ControllerIconSprite.XB1_X;
					break;
				case RuntimePlatform.PS4:
					returnVal = ControllerIconSprite.PS4_Square_Button;
					break;
				case RuntimePlatform.Switch:
					returnVal = ControllerIconSprite.SWITCH_X;
					break;
				case RuntimePlatform.XboxOne:
				default:
					returnVal = ControllerIconSprite.XB1_X;
					break;
			}

			return (int)returnVal;
		}

		public static int GetSpriteIndex_YButton()
		{
			ControllerIconSprite returnVal;

			switch (Application.platform)
			{
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsEditor:
					string str = PlayerController.Instance.inputController.player.controllers.Joysticks.FirstOrDefault<Joystick>()?.name ?? "unknown";
					if (str.Contains("Dual Shock") || str.Contains("DualShock"))
					{
						returnVal = ControllerIconSprite.PS4_Triangle_Button;
						break;
					}
					returnVal = ControllerIconSprite.XB1_Y;
					break;
				case RuntimePlatform.PS4:
					returnVal = ControllerIconSprite.PS4_Triangle_Button;
					break;
				case RuntimePlatform.Switch:
					returnVal = ControllerIconSprite.SWITCH_Y;
					break;
				case RuntimePlatform.XboxOne:
				default:
					returnVal = ControllerIconSprite.XB1_Y;
					break;
			}

			return (int)returnVal;
		}
	}
}

