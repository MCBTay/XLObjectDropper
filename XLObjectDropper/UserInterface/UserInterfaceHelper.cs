using GameManagement;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityModManagerNet;
using XLObjectDropper.GameManagement;
using XLObjectDropper.UI;

namespace XLObjectDropper.UserInterface
{
	public static class UserInterfaceHelper
	{
		public static GameObject UserInterface { get; set; }
		
		public static void LoadUserInterface()
		{
			if (UIManager.Instance == null)
			{
				UserInterface = AssetBundleHelper.LoadUIBundle();

				OptionsMenuController.OptionsMenu = UserInterface.GetComponentInChildren<OptionsMenuUI>(true);
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
					GameStateMachine.Instance.RequestTransitionTo(typeof(ObjectMovementState));
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
