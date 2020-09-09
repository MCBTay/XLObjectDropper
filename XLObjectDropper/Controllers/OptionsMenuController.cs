using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityModManagerNet;
using XLObjectDropper.UI;
using XLObjectDropper.EventStack;

namespace XLObjectDropper.Controllers
{
	public class OptionsMenuController : MonoBehaviour
	{
		public static OptionsMenuUI OptionsMenu { get; set; }

		private void Awake()
		{
			OptionsMenu.Snapping.GetComponent<Toggle>().isOn = Settings.Instance.Snapping;
			OptionsMenu.Sensitivity.GetComponent<Slider>().value = Settings.Instance.Sensitivity;
		}

		private void OnEnable()
		{
			OptionsMenu.Snapping.GetComponent<Toggle>().isOn = Settings.Instance.Snapping;
			OptionsMenu.Sensitivity.GetComponent<Slider>().value = Settings.Instance.Sensitivity;

			AddListeners();

			OptionsMenu.EnableUndoButton(EventStack.EventStack.Instance.UndoQueue.Count > 0);
			OptionsMenu.EnableRedoButton(EventStack.EventStack.Instance.RedoQueue.Count > 0);

			EventSystem.current.SetSelectedGameObject(OptionsMenu.Snapping);
			OptionsMenu.Snapping.GetComponent<Toggle>().OnSelect(null);
		}

		private void AddListeners()
		{
			RemoveListeners();

			OptionsMenu.SnappingValueChanged += SnappingValueChanged;
			OptionsMenu.SensitivityValueChanged += SensitivityValueChanged;
			OptionsMenu.UndoClicked += UndoClicked;
			OptionsMenu.RedoClicked += RedoClicked;
			OptionsMenu.SaveClicked += SaveClicked;
			OptionsMenu.LoadClicked += LoadClicked;
		}

		private void RemoveListeners()
		{
			OptionsMenu.SnappingValueChanged -= SnappingValueChanged;
			OptionsMenu.SensitivityValueChanged -= SensitivityValueChanged;
			OptionsMenu.UndoClicked -= UndoClicked;
			OptionsMenu.RedoClicked -= RedoClicked;
			OptionsMenu.SaveClicked -= SaveClicked;
			OptionsMenu.LoadClicked -= LoadClicked;
		}

		private void OnDisable()
		{
			RemoveListeners();
		}

		private static void SnappingValueChanged(bool value)
		{
			Settings.Instance.Snapping = value;
			SaveManager.Instance.SaveSettings();
		}

		private static void SensitivityValueChanged(float value)
		{
			Settings.Instance.Sensitivity = value;
			SaveManager.Instance.SaveSettings();
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
			SaveManager.Instance.SaveCurrentSpawnables();
		}

		private static void LoadClicked()
		{
			SaveManager.Instance.LoadSpawnables();
		}
	}
}
