using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using XLObjectDropper.UI;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Controllers
{
	public class OptionsMenuController : MonoBehaviour
	{
		public static OptionsMenuUI OptionsMenu { get; set; }

		[HideInInspector] public event UnityAction SaveLoaded = () => { };

		private GameObject LoadSavedGameObject;
		private LoadSavedController LoadSavedController;
		public bool LoadSavedOpen => LoadSavedGameObject != null && LoadSavedGameObject.activeInHierarchy;

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

			EventSystem.current.SetSelectedGameObject(OptionsMenu.Sensitivity.gameObject);
			OptionsMenu.Sensitivity.OnSelect(null);
		}

		private void Update()
		{
			OptionsMenu.EnableUndoButton(EventStack.EventStack.Instance.UndoQueue.Count > 0);
			OptionsMenu.EnableRedoButton(EventStack.EventStack.Instance.RedoQueue.Count > 0);
			OptionsMenu.EnableSaveButton(SpawnableManager.SpawnedObjects.Any() && Utilities.SaveManager.Instance.HasUnsavedChanges);

			if (UIManager.Instance.Player.GetButtonDown("B") && LoadSavedOpen)
			{
				DestroyLoadSaved();
			}
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

		private void SensitivityValueChanged(float value)
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

		private void UndoClicked()
		{
			EventStack.EventStack.Instance.UndoAction();
		}

		private void RedoClicked()
		{
			EventStack.EventStack.Instance.RedoAction();
		}

		private void SaveClicked()
		{
			Utilities.SaveManager.Instance.SaveCurrentSpawnables();
		}

		private void LoadClicked()
		{
			CreateLoadSaved();
		}

		private void CreateLoadSaved()
		{
			UISounds.Instance?.PlayOneShotSelectMajor();

			LoadSavedGameObject = new GameObject();
			LoadSavedController = LoadSavedGameObject.AddComponent<LoadSavedController>();
			LoadSavedController.SaveLoaded += () => SaveLoaded.Invoke();
			LoadSavedGameObject.SetActive(true);
		}

		private void DestroyLoadSaved()
		{
			if (LoadSavedGameObject == null || LoadSavedController == null) return;

			LoadSavedController.LoadSavedUI.DestroyUnsavedChangesDialog();
			LoadSavedController.LoadSavedUI.gameObject.SetActive(false);
			

			LoadSavedGameObject.SetActive(false);

			DestroyImmediate(LoadSavedController);
			DestroyImmediate(LoadSavedGameObject);
		}
	}
}
