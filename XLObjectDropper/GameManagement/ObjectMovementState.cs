using GameManagement;
using UnityEngine;

namespace XLObjectDropper.GameManagement
{
	public class ObjectMovementState : GameState
	{
		private GameObject ObjectMovementControllerGameObject { get; set; }
		public ObjectMovementController ObjectMovementController { get; set; }
		public static ObjectMovementState Instance { get; private set; }

		public ObjectMovementState()
		{
			Instance = this;

			availableTransitions = new[] { typeof(PauseState), typeof(ObjectSelectionState) };
		}

		public override void OnEnter()
		{
			ObjectMovementControllerGameObject = new GameObject();
			ObjectMovementController = ObjectMovementControllerGameObject.AddComponent<ObjectMovementController>();

			Time.timeScale = 1.0f;

			GameStateMachine.Instance.PauseObject.SetActive(false);
			GameStateMachine.Instance.PinObject.SetActive(true);
			PlayerController.Instance.EnablePuppetMaster(false, true);
			ObjectMovementControllerGameObject.SetActive(true);
			GameStateMachine.Instance.PlayObject.SetActive(false);
		}

		public override void OnExit()
		{
			Time.timeScale = 0.0f;
			GameStateMachine.Instance.PauseObject.SetActive(true);

			GameStateMachine.Instance.PinObject.SetActive(false);
			ObjectMovementControllerGameObject.SetActive(false);
			GameStateMachine.Instance.PlayObject.SetActive(true);
			PlayerController.Instance.IsExitingPinState = true;
			PlayerController.Instance.EnablePuppetMaster(true, false);
		}

		public override void OnUpdate()
		{
			if (!PlayerController.Instance.inputController.player.GetButtonDown("B"))
				return;

			GameStateMachine.Instance.PinObject.SetActive(false);
			RequestTransitionBack();
		}
	}
}
