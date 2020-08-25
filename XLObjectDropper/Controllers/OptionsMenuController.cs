using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using XLObjectDropper.UI;

namespace XLObjectDropper.Controllers
{
	public class OptionsMenuController : MonoBehaviour
	{
		public static OptionsMenuUI OptionsMenu { get; set; }

		private void Awake()
		{
			
		}

		private void OnEnable()
		{
			OptionsMenu.Snapping.GetComponent<Toggle>().isOn = Settings.Instance.Snapping;
			OptionsMenu.Sensitivity.GetComponent<Slider>().value = Settings.Instance.Sensitivity;
			
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
			UnityModManager.Logger.Log("XLObjectDropper.UserInterfaceHelper: Undo clicked.");
		}

		private static void RedoClicked()
		{
			UnityModManager.Logger.Log("XLObjectDropper.UserInterfaceHelper: Redo clicked.");
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
