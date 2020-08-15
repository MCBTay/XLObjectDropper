using GameManagement;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using XLObjectDropper.GameManagement;

namespace XLObjectDropper.UserInterface
{
	public static class UserInterfaceHelper
	{
		public static void CreateObjectDropperButton()
		{
			var menuButtons = GameStateMachine.Instance.PauseObject.GetComponentsInChildren<MenuButton>();

			if (menuButtons != null && menuButtons.Any())
			{
				var buttonToClone = menuButtons.ElementAt(6);

				var newButton = Object.Instantiate(buttonToClone, buttonToClone.gameObject.transform);

				if (newButton == null) return;

				newButton.Label.SetText("Object Dropper, yo");
				newButton.gameObject.SetActive(true);

				newButton.onClick.SetPersistentListenerState(0, UnityEventCallState.Off);
				newButton.onClick.AddListener(() =>
				{
					GameStateMachine.Instance.RequestTransitionTo(typeof(ObjectMovementState));
				});
			}

		}
	}
}
