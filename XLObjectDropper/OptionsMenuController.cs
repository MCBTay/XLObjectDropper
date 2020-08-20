using UnityEngine;
using UnityModManagerNet;
using XLObjectDropper.UI;

namespace XLObjectDropper
{
	public class OptionsMenuController : MonoBehaviour
	{
		public static OptionsMenuUI OptionsMenu { get; set; }

		private void Awake()
		{
			
		}

		private void OnEnable()
		{
			OptionsMenu.SnappingValueChanged += SnappingValueChanged;
			OptionsMenu.SensitivityValueChanged += SensitivityValueChanged;
			OptionsMenu.UndoClicked += UndoClicked;
			OptionsMenu.RedoClicked += RedoClicked;
			OptionsMenu.SaveClicked += SaveClicked;
			OptionsMenu.LoadClicked += LoadClicked;

			OptionsMenu.gameObject.SetActive(true);
		}

		private void OnDisable()
		{
			OptionsMenu.SnappingValueChanged -= SnappingValueChanged;
			OptionsMenu.SensitivityValueChanged -= SensitivityValueChanged;
			OptionsMenu.UndoClicked -= UndoClicked;
			OptionsMenu.RedoClicked -= RedoClicked;
			OptionsMenu.SaveClicked -= SaveClicked;
			OptionsMenu.LoadClicked -= LoadClicked;

			OptionsMenu.gameObject.SetActive(false);
		}

		private void Update()
		{

		}

		private static void SnappingValueChanged(bool value)
		{
			UnityModManager.Logger.Log("XLObjectDropper.UserInterfaceHelper: Snapping Value Changed - " + value);
		}

		private static void SensitivityValueChanged(float value)
		{
			UnityModManager.Logger.Log("XLObjectDropper.UserInterfaceHelper: Sensitivity Value Changed - " + value);
		}

		private static void UndoClicked()
		{
			UnityModManager.Logger.Log("XLObjectDropper.UserInterfaceHelper: Undo clicked.");
		}

		private static void RedoClicked()
		{
			UnityModManager.Logger.Log("XLObjectDropper.UserInterfaceHelper: Redo clicked.");
		}

		private static void SaveClicked()
		{
			UnityModManager.Logger.Log("XLObjectDropper.UserInterfaceHelper: Save clicked.");
			SaveManager.Instance.SaveCurrentSpawnables();
		}

		private static void LoadClicked()
		{
			UnityModManager.Logger.Log("XLObjectDropper.UserInterfaceHelper: Load clicked.");
			SaveManager.Instance.LoadSpawnables();
		}
	}
}
