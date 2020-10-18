using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using XLObjectDropper.EventStack.Events;
using XLObjectDropper.UI;
using XLObjectDropper.UI.Controls;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Controllers
{
	public class OptionsMenuController : MonoBehaviour
	{
		public static OptionsMenuUI OptionsMenu { get; set; }

		public event UnityAction SaveLoaded = () => { };
		public event UnityAction Saved = () => { };
		public event UnityAction SaveCancelled = () => { };

		private GameObject LoadSavedGameObject;
		private LoadSavedController LoadSavedController;
		public bool LoadSavedOpen => LoadSavedGameObject != null && LoadSavedGameObject.activeInHierarchy;

		private GameObject SaveGameObject;
		public SaveController SaveController;
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
			OptionsMenu.UndoButton.interactable = EventStack.EventStack.Instance.UndoQueue.Count > 0;
			OptionsMenu.RedoButton.interactable = EventStack.EventStack.Instance.RedoQueue.Count > 0;
			OptionsMenu.SaveButton.interactable = SpawnableManager.SpawnedObjects.Any() && Utilities.SaveManager.Instance.HasUnsavedChanges;
			OptionsMenu.ClearAllButton.interactable = SpawnableManager.SpawnedObjects.Any();

			if (UIManager.Instance.Player.GetButtonDown("B"))
			{
				if (LoadSavedOpen)
				{
					DestroyLoadSaved();
				}
				if (SaveOpen)
				{
					DestroySave();
				}
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
			OptionsMenu.ClearAllButton.onClick.AddListener(ClearAllClicked);
			OptionsMenu.SaveButton.onClick.AddListener(SaveClicked);
			OptionsMenu.LoadButton.onClick.AddListener(LoadClicked);

			OptionsMenu.Sensitivity.gameObject.GetComponent<SelectableControl>().onSelect += Selected;
			OptionsMenu.InvertCamControl.gameObject.GetComponent<SelectableControl>().onSelect += Selected;
			OptionsMenu.ShowGrid.gameObject.GetComponent<SelectableControl>().onSelect += Selected;
			OptionsMenu.UndoButton.gameObject.GetComponent<SelectableControl>().onSelect += Selected;
			OptionsMenu.RedoButton.gameObject.GetComponent<SelectableControl>().onSelect += Selected;
			OptionsMenu.ClearAllButton.gameObject.GetComponent<SelectableControl>().onSelect += Selected;
			OptionsMenu.SaveButton.gameObject.GetComponent<SelectableControl>().onSelect += Selected;
			OptionsMenu.LoadButton.gameObject.GetComponent<SelectableControl>().onSelect += Selected;
		}

		private void Selected()
		{
			UISounds.Instance?.PlayOneShotSelectionChange();
		}

		private void RemoveListeners()
		{
			OptionsMenu.Sensitivity.onValueChanged.RemoveListener(SensitivityValueChanged);
			OptionsMenu.InvertCamControl.onValueChanged.RemoveListener(InvertCamControlValueChanged);
			OptionsMenu.ShowGrid.onValueChanged.RemoveListener(ShowGridValueChanged);
			OptionsMenu.UndoButton.onClick.RemoveListener(UndoClicked);
			OptionsMenu.RedoButton.onClick.RemoveListener(RedoClicked);
			OptionsMenu.ClearAllButton.onClick.RemoveListener(ClearAllClicked);
			OptionsMenu.SaveButton.onClick.RemoveListener(SaveClicked);
			OptionsMenu.LoadButton.onClick.RemoveListener(LoadClicked);

			OptionsMenu.Sensitivity.gameObject.GetComponent<SelectableControl>().onSelect -= Selected;
			OptionsMenu.InvertCamControl.gameObject.GetComponent<SelectableControl>().onSelect -= Selected;
			OptionsMenu.ShowGrid.gameObject.GetComponent<SelectableControl>().onSelect -= Selected;
			OptionsMenu.UndoButton.gameObject.GetComponent<SelectableControl>().onSelect -= Selected;
			OptionsMenu.RedoButton.gameObject.GetComponent<SelectableControl>().onSelect -= Selected;
			OptionsMenu.ClearAllButton.gameObject.GetComponent<SelectableControl>().onSelect -= Selected;
			OptionsMenu.SaveButton.gameObject.GetComponent<SelectableControl>().onSelect -= Selected;
			OptionsMenu.LoadButton.gameObject.GetComponent<SelectableControl>().onSelect -= Selected;
		}

		private void OnDisable()
		{
			RemoveListeners();
		}

		private void SensitivityValueChanged(float value)
		{
			UISounds.Instance?.PlayOneShotSelectionChange();

			Settings.Instance.Sensitivity = value;
			Settings.Instance.Save();
		}

		private static void InvertCamControlValueChanged(bool value)
		{
			UISounds.Instance?.PlayOneShotSelectionChange();

			Settings.Instance.InvertCamControl = value;
			Settings.Instance.Save();
		}

		private static void ShowGridValueChanged(bool value)
		{
			UISounds.Instance?.PlayOneShotSelectionChange();

			Settings.Instance.ShowGrid = value;
			Settings.Instance.Save();

			if (ObjectMovementController.Instance.GridOverlay != null)
			{
				ObjectMovementController.Instance.GridOverlay.enabled = value;
			}
		}

		private void UndoClicked()
		{
			EventStack.EventStack.Instance.UndoAction();
		}

		private void RedoClicked()
		{
			EventStack.EventStack.Instance.RedoAction();
		}

		private void ClearAllClicked()
		{
			if (SpawnableManager.SpawnedObjects == null || !SpawnableManager.SpawnedObjects.Any()) return;

			// Should we maybe show an 'are you sure?' dialog
			var gameObjects = SpawnableManager.SpawnedObjects.Select(x => x.SpawnedInstance).ToList();
			var batchObjectDeletedEvent = new BatchObjectDeletedEvent(gameObjects);
			batchObjectDeletedEvent.AddToUndoStack();

			SpawnableManager.DeleteSpawnedObjects();
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
			SaveController.Saved += SaveControllerSaveClicked;
			SaveController.SaveCancelled += SaveControllerCancelled;
			SaveGameObject.SetActive(true);
		}

		private void SaveControllerSaveClicked()
		{
			Saved.Invoke();
		}

		private void SaveControllerCancelled()
		{
			SaveCancelled.Invoke();
		}

		public void DestroySave()
		{
			if (SaveGameObject == null || SaveController == null) return;

			SaveController.Saved -= SaveControllerSaveClicked;
			SaveController.SaveCancelled -= SaveControllerCancelled;

			SaveGameObject.SetActive(false);

			DestroyImmediate(SaveController);
			DestroyImmediate(SaveGameObject);
		}
	}
}
