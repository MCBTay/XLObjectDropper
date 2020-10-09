using System.Linq;
using GameManagement;
using UnityEngine;
using UnityModManagerNet;
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

		private bool XLGraphicsWasEnabled = false;
		private bool XXLGraphicUtilsWasEnabled = false;

		public override void OnEnter()
		{
			var xlGraphics = UnityModManager.modEntries.FirstOrDefault(x => x.Info.Id == "XLGraphics");
			if (xlGraphics != null && xlGraphics.Enabled)
			{
				XLGraphicsWasEnabled = true;
				xlGraphics.OnToggle(xlGraphics, false);
			}

			var xxlGraphicUtils = UnityModManager.modEntries.FirstOrDefault(x => x.Info.Id == "XXLGraphicUtils");
			if (xxlGraphicUtils != null && xxlGraphicUtils.Enabled)
			{
				XXLGraphicUtilsWasEnabled = true;
				xxlGraphicUtils.OnToggle(xxlGraphicUtils, false);
			}

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
			ObjectDropperController.enabled = false;
			Object.DestroyImmediate(ObjectDropperControllerGameObject);

			GameStateMachine.Instance.PlayObject.SetActive(true);
			PlayerController.Instance.EnablePuppetMaster(true, false);

			var xlGraphics = UnityModManager.modEntries.FirstOrDefault(x => x.Info.Id == "XLGraphics");
			if (xlGraphics != null && XLGraphicsWasEnabled)
			{
				xlGraphics.OnToggle(xlGraphics, true);
				XLGraphicsWasEnabled = false;
			}

			var xxlGraphicUtils = UnityModManager.modEntries.FirstOrDefault(x => x.Info.Id == "XXLGraphicUtils");
			if (xxlGraphicUtils != null && XXLGraphicUtilsWasEnabled)
			{
				xxlGraphicUtils.OnToggle(xxlGraphicUtils, true);
				XXLGraphicUtilsWasEnabled = false;
			}
		}
	}
}
