using GameManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.HighDefinition;
using XLObjectDropper.Controllers;
using XLObjectDropper.GameManagement;
using XLObjectDropper.UI;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.UserInterface
{
	public static class UserInterfaceHelper
	{
		public static GameObject UserInterface { get; set; }

		private static GameObject CustomPass;
		public static CustomPassVolume CustomPassVolume;
		public static List<CustomPass> CustomPasses;

		public static void LoadUserInterface()
		{
			if (UIManager.Instance == null)
			{
				AssetBundleHelper.LoadUIBundle();
				
				UserInterface = Object.Instantiate(AssetBundleHelper.UIPrefab);
				Object.DontDestroyOnLoad(UserInterface);

				UserInterface.SetActive(false);

				UIManager.Instance.Player = PlayerController.Instance.inputController.player;

				ObjectMovementController.MovementUI = UserInterface.GetComponentInChildren<ObjectPlacementUI>(true);
				OptionsMenuController.OptionsMenu = UserInterface.GetComponentInChildren<OptionsMenuUI>(true);
				ObjectSelectionController.ObjectSelection = UserInterface.GetComponentInChildren<ObjectSelectionUI>(true);
				QuickMenuController.QuickMenu = UserInterface.GetComponentInChildren<QuickMenuUI>(true);

				CustomPass = Object.Instantiate(AssetBundleHelper.CustomPassPrefab);
				Object.DontDestroyOnLoad(CustomPass);

				CustomPassVolume = CustomPass.GetComponent<CustomPassVolume>();
			}
		}

		#region Object Dropper Button
		private static MenuButton ObjectDropperButton { get; set; }

		public static void CreateObjectDropperButton()
		{
			var menuButtons = GameStateMachine.Instance.PauseObject.GetComponentsInChildren<MenuButton>();

			if (menuButtons != null && menuButtons.Any())
			{
				var buttonToClone = menuButtons.ElementAt(6);

				ObjectDropperButton = Object.Instantiate(buttonToClone, buttonToClone.gameObject.transform);

				if (ObjectDropperButton == null) return;

				ObjectDropperButton.Label.SetText("Object Dropper");
				ObjectDropperButton.gameObject.SetActive(true);

				ObjectDropperButton.onClick.SetPersistentListenerState(0, UnityEventCallState.Off);
				ObjectDropperButton.onClick.AddListener(() =>
				{
					GameStateMachine.Instance.RequestTransitionTo(typeof(ObjectDropperState));
				});
			}
		}

		public static void DestroyObjectDropperButton()
		{
			if (ObjectDropperButton != null)
			{
				ObjectDropperButton.gameObject.SetActive(false);
				Object.Destroy(ObjectDropperButton);
			}
		}
		#endregion
	}
}
