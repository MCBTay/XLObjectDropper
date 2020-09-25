using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLObjectDropper.UI.Menus;

namespace XLObjectDropper.Controllers
{
	public class OptionsMenuController : MonoBehaviour
	{
		public static OptionsMenuUI OptionsMenu { get; set; }

		private void Awake()
		{
			OptionsMenu.Sensitivity.value = Settings.Instance.Sensitivity;
			OptionsMenu.InvertCamControl.isOn = Settings.Instance.InvertCamControl;
			OptionsMenu.ShowGrid.isOn = Settings.Instance.ShowGrid;
		}

		private void OnEnable()
		{
			OptionsMenu.Sensitivity.value = Settings.Instance.Sensitivity;
			OptionsMenu.InvertCamControl.isOn = Settings.Instance.InvertCamControl;
			OptionsMenu.ShowGrid.isOn = Settings.Instance.ShowGrid;

			AddListeners();

			OptionsMenu.EnableUndoButton(EventStack.EventStack.Instance.UndoQueue.Count > 0);
			OptionsMenu.EnableRedoButton(EventStack.EventStack.Instance.RedoQueue.Count > 0);

			EventSystem.current.SetSelectedGameObject(OptionsMenu.Sensitivity.gameObject);
			OptionsMenu.Sensitivity.OnSelect(null);
		}

		private void AddListeners()
		{
			RemoveListeners();

			OptionsMenu.SensitivityValueChanged += SensitivityValueChanged;
			OptionsMenu.InvertCamControlValueChanged += InvertCamControlValueChanged;
			OptionsMenu.ShowGridValueChanged += ShowGridValueChanged;
			OptionsMenu.UndoClicked += UndoClicked;
			OptionsMenu.RedoClicked += RedoClicked;
			OptionsMenu.SaveClicked += SaveClicked;
			OptionsMenu.LoadClicked += LoadClicked;
		}

		private void RemoveListeners()
		{
			OptionsMenu.SensitivityValueChanged -= SensitivityValueChanged;
			OptionsMenu.InvertCamControlValueChanged -= InvertCamControlValueChanged;
			OptionsMenu.ShowGridValueChanged -= ShowGridValueChanged;
			OptionsMenu.UndoClicked -= UndoClicked;
			OptionsMenu.RedoClicked -= RedoClicked;
			OptionsMenu.SaveClicked -= SaveClicked;
			OptionsMenu.LoadClicked -= LoadClicked;
		}

		private void OnDisable()
		{
			RemoveListeners();
		}

		private static void SensitivityValueChanged(float value)
		{
			Settings.Instance.Sensitivity = value;
			Utilities.SaveManager.Instance.SaveSettings();
		}

		private static void InvertCamControlValueChanged(bool value)
		{
			Settings.Instance.InvertCamControl = value;
			Utilities.SaveManager.Instance.SaveSettings();
		}

		private static void ShowGridValueChanged(bool value)
		{
			Settings.Instance.ShowGrid = value;
			Utilities.SaveManager.Instance.SaveSettings();
		}

		private static void UndoClicked()
		{
			EventStack.EventStack.Instance.UndoAction();

			OptionsMenu.EnableUndoButton(EventStack.EventStack.Instance.UndoQueue.Count > 0);
			OptionsMenu.EnableRedoButton(EventStack.EventStack.Instance.RedoQueue.Count > 0);
		}

		private static void RedoClicked()
		{
			EventStack.EventStack.Instance.RedoAction();

			OptionsMenu.EnableUndoButton(EventStack.EventStack.Instance.UndoQueue.Count > 0);
			OptionsMenu.EnableRedoButton(EventStack.EventStack.Instance.RedoQueue.Count > 0);
		}

		private static void SaveClicked()
		{
			Utilities.SaveManager.Instance.SaveCurrentSpawnables();
		}

		private static void LoadClicked()
		{
			Utilities.SaveManager.Instance.LoadSpawnables();
		}
	}
}
