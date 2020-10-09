using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityModManagerNet;
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

		private GameObject SaveGameObject;
		private SaveController SaveController;
		public bool SaveOpen => SaveGameObject != null && SaveGameObject.activeInHierarchy;

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

			if (UIManager.Instance.Player.GetButtonDown("B"))
			{
				if (LoadSavedOpen) DestroyLoadSaved();
				if (SaveOpen) DestroySave();
			}
		}

		private void AddListeners()
		{
			RemoveListeners();

			OptionsMenu.Sensitivity.onValueChanged.AddListener(SensitivityValueChanged);
			OptionsMenu.InvertCamControl.onValueChanged.AddListener(InvertCamControlValueChanged);
			OptionsMenu.ShowGrid.onValueChanged.AddListener(ShowGridValueChanged);
			OptionsMenu.UndoButton.onClick.AddListener(UndoClicked);
			OptionsMenu.RedoButton.onClick.AddListener(RedoClicked);
			OptionsMenu.SaveButton.onClick.AddListener(SaveClicked);
			OptionsMenu.LoadButton.onClick.AddListener(LoadClicked);
		}

		private void RemoveListeners()
		{
			OptionsMenu.Sensitivity.onValueChanged.RemoveListener(SensitivityValueChanged);
			OptionsMenu.InvertCamControl.onValueChanged.RemoveListener(InvertCamControlValueChanged);
			OptionsMenu.ShowGrid.onValueChanged.RemoveListener(ShowGridValueChanged);
			OptionsMenu.UndoButton.onClick.RemoveListener(UndoClicked);
			OptionsMenu.RedoButton.onClick.RemoveListener(RedoClicked);
			OptionsMenu.SaveButton.onClick.RemoveListener(SaveClicked);
			OptionsMenu.LoadButton.onClick.RemoveListener(LoadClicked);
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
			CreateSave();
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

		private void CreateSave()
		{
			UISounds.Instance?.PlayOneShotSelectMajor();

			SaveGameObject = new GameObject();
			SaveController = SaveGameObject.AddComponent<SaveController>();

			//SaveController.SaveLoaded += () => SaveLoaded.Invoke();
			SaveGameObject.SetActive(true);
		}

		private void DestroySave()
		{
			if (SaveGameObject == null || SaveController == null) return;
			SaveController.SaveUI.gameObject.SetActive(false);
			SaveGameObject.SetActive(false);

			DestroyImmediate(SaveController);
			DestroyImmediate(SaveGameObject);
		}
	}
}
