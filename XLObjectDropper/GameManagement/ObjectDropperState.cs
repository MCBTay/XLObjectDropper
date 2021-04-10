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
		}

		private bool XLGraphicsWasEnabled = false;
		private bool XXLGraphicUtilsWasEnabled = false;

		public override void OnEnter()
		{
			ObjectDropperControllerGameObject = new GameObject();
			ObjectDropperController = ObjectDropperControllerGameObject.AddComponent<ObjectDropperController>();

			Time.timeScale = 1.0f;

			GameStateMachine.Instance.PauseObject.SetActive(false);
			PlayerController.Instance.EnablePuppetMaster(false, true);
			ObjectDropperControllerGameObject.SetActive(true);
			GameStateMachine.Instance.PlayObject.SetActive(false);
			
			GameStateMachine.Instance.SemiTransparentLayer.SetActive(false);
		}

		public override void OnExit()
		{
			Time.timeScale = 0.0f;
			GameStateMachine.Instance.PauseObject.SetActive(true);
			
			ObjectDropperControllerGameObject.SetActive(false);
			ObjectDropperController.enabled = false;
			Object.DestroyImmediate(ObjectDropperControllerGameObject);

			GameStateMachine.Instance.PlayObject.SetActive(true);
			PlayerController.Instance.EnablePuppetMaster(true, false);

			GameStateMachine.Instance.SemiTransparentLayer.SetActive(true);
		}
	}
}
