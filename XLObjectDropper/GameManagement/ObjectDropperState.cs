using GameManagement;
using UnityEngine;
using XLObjectDropper.Controllers;

namespace XLObjectDropper.GameManagement
{
	public class ObjectDropperState : GameState
	{
		private GameObject ObjectDropperControllerGameObject;
		private ObjectDropperController ObjectDropperController;

		public static ObjectDropperState Instance { get; private set; }

		public ObjectDropperState()
		{
			Instance = this;

			availableTransitions = new[]
			{
				typeof(PauseState)
			};
		}

		public override void OnEnter()
		{
			ObjectDropperControllerGameObject = new GameObject();
			ObjectDropperController = ObjectDropperControllerGameObject.AddComponent<ObjectDropperController>();

			Time.timeScale = 1.0f;

			GameStateMachine.Instance.PauseObject.SetActive(false);
			PlayerController.Instance.EnablePuppetMaster(false, true);
			ObjectDropperControllerGameObject.SetActive(true);
			GameStateMachine.Instance.PlayObject.SetActive(false);
		}

		public override void OnExit()
		{
			Time.timeScale = 0.0f;
			GameStateMachine.Instance.PauseObject.SetActive(true);
			ObjectDropperControllerGameObject.SetActive(false);
			GameStateMachine.Instance.PlayObject.SetActive(true);
			PlayerController.Instance.EnablePuppetMaster(true, false);
		}
	}
}
