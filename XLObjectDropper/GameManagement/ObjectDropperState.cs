﻿using GameManagement;
using UnityEngine;
using XLObjectDropper.Controllers;

namespace XLObjectDropper.GameManagement
{
	public class ObjectDropperState : GameState
	{
		private GameObject ObjectMovementControllerGameObject { get; set; }
		public ObjectMovementController ObjectMovementController { get; set; }
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
			ObjectMovementControllerGameObject = new GameObject();
			ObjectMovementController = ObjectMovementControllerGameObject.AddComponent<ObjectMovementController>();

			Time.timeScale = 1.0f;

			GameStateMachine.Instance.PauseObject.SetActive(false);
			PlayerController.Instance.EnablePuppetMaster(false, true);
			ObjectMovementControllerGameObject.SetActive(true);
			GameStateMachine.Instance.PlayObject.SetActive(false);
		}

		public override void OnExit()
		{
			Time.timeScale = 0.0f;
			GameStateMachine.Instance.PauseObject.SetActive(true);
			ObjectMovementControllerGameObject.SetActive(false);
			GameStateMachine.Instance.PlayObject.SetActive(true);
			PlayerController.Instance.EnablePuppetMaster(true, false);
		}
	}
}
