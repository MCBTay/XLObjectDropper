using GameManagement;
using UnityEngine;
using XLObjectDropper.Controllers;

namespace XLObjectDropper.GameManagement
{
	public class ObjectSelectionState : GameState
	{
		private GameObject ObjectSelectionControllerGameObject { get; set; }
		private ObjectSelectionController ObjectSelectionController { get; set; }
		public static ObjectSelectionState Instance { get; private set; }

		public ObjectSelectionState()
		{
			Instance = this;

			availableTransitions = new[] { typeof(ObjectMovementState) };
		}

		public override void OnEnter()
		{
			ObjectSelectionControllerGameObject = new GameObject();
			ObjectSelectionController = ObjectSelectionControllerGameObject.AddComponent<ObjectSelectionController>();

			Time.timeScale = 0.0f;

			GameStateMachine.Instance.PauseObject.SetActive(true);
			PlayerController.Instance.EnablePuppetMaster(false, true);
			ObjectSelectionControllerGameObject.SetActive(true);
			GameStateMachine.Instance.PlayObject.SetActive(false);
		}

		public override void OnExit()
		{
			Time.timeScale = 1.0f;

			GameStateMachine.Instance.PauseObject.SetActive(true);
			PlayerController.Instance.EnablePuppetMaster(true, false);
			ObjectSelectionControllerGameObject.SetActive(false);
			GameStateMachine.Instance.PlayObject.SetActive(true);
		}

		public override void OnUpdate()
		{
			if (!PlayerController.Instance.inputController.player.GetButtonDown("B"))
				return;

			RequestTransitionBack();
		}
	}
}
