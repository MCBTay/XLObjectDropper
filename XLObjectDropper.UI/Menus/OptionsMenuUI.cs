using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLObjectDropper.UI.Controls;
using XLObjectDropper.UI.Controls.ControllerButtons;

namespace XLObjectDropper.UI.Menus
{
	public class OptionsMenuUI : MonoBehaviour
	{
		[Header("Options Menu Elements")]
		public GameObject MainUI;
		[Space(10)]
		public Slider Sensitivity;
		public Toggle InvertCamControl;
		public Toggle ShowGrid;

		[Space(10)]
		public Button UndoButton;
		public Button RedoButton;
		public Button SaveButton;
		public Button LoadButton;

		[Space(10)]
		public BottomRowController BottomRow;

		public GameObject LoadSavedUI;
		public Animator Animator;

		[HideInInspector] public event UnityAction<float> SensitivityValueChanged = (x) => { };
		[HideInInspector] public event UnityAction<bool> InvertCamControlValueChanged = (x) => { };
		[HideInInspector] public event UnityAction<bool> ShowGridValueChanged = (x) => { };
		[HideInInspector] public event UnityAction UndoClicked = () => { };
		[HideInInspector] public event UnityAction RedoClicked = () => { };
		[HideInInspector] public event UnityAction SaveClicked = () => { };
		[HideInInspector] public event UnityAction LoadClicked = () => { };

		private void OnEnable()
		{
			SetDefaultState(true);
			Animator.Play("SlideIn");
		}

		private void Start()
		{
			SetDefaultState(true);
		}

		private void Update()
		{
			if (UIManager.Instance.Player.GetButtonDown("B") && LoadSavedUI != null && LoadSavedUI.activeInHierarchy)
			{
				StartCoroutine(DisableLoadSavedUI());

				//SetAllInteractable(true);

				EventSystem.current.SetSelectedGameObject(LoadButton.gameObject);
			}
		}

		public IEnumerator DisableLoadSavedUI()
		{
			LoadSavedUI.GetComponent<LoadSavedUI>().Animator.Play("SlideOut");
			yield return new WaitForSeconds(0.2f);
			LoadSavedUI.SetActive(false);
		}

		private void SetDefaultState(bool isEnabled)
		{
			LoadSavedUI.SetActive(false);

			SetDefaultState(Sensitivity, isEnabled);
			SetDefaultState(InvertCamControl, isEnabled);
			SetDefaultState(ShowGrid, isEnabled);
			SetDefaultState(UndoButton, isEnabled);
			SetDefaultState(RedoButton, isEnabled);
			SetDefaultState(SaveButton, isEnabled);
			SetDefaultState(LoadButton, isEnabled);

			if (isEnabled)
			{
				Sensitivity.onValueChanged.AddListener(delegate { SensitivityValueChanged.Invoke(Sensitivity.GetComponent<Slider>().value); });
				InvertCamControl.onValueChanged.AddListener(delegate { InvertCamControlValueChanged.Invoke(InvertCamControl.GetComponent<Toggle>().isOn); });
				ShowGrid.onValueChanged.AddListener(delegate { ShowGridValueChanged.Invoke(ShowGrid.GetComponent<Toggle>().isOn); });
				UndoButton.onClick.AddListener(delegate { UndoClicked.Invoke(); });
				RedoButton.onClick.AddListener(delegate { RedoClicked.Invoke(); });
				SaveButton.onClick.AddListener(delegate { SaveClicked.Invoke(); });
				
				LoadButton.onClick.AddListener(delegate
				{
					LoadClicked.Invoke();
					LoadSavedUI.SetActive(true);

					SetAllInteractable(false);
				});
			}
		}

		private void SetDefaultState(Slider slider, bool isEnabled)
		{
			slider.onValueChanged.RemoveAllListeners();
			slider.interactable = true;
			slider.gameObject.SetActive(isEnabled);
		}

		private void SetDefaultState(Toggle toggle, bool isEnabled)
		{
			toggle.onValueChanged.RemoveAllListeners();
			toggle.interactable = true;
			toggle.gameObject.SetActive(isEnabled);
		}

		private void SetDefaultState(Button button, bool isEnabled)
		{
			button.onClick.RemoveAllListeners();
			button.interactable = true;
			button.gameObject.SetActive(isEnabled);
		}

		private void SetAllInteractable(bool interactable)
		{
			Sensitivity.interactable = interactable;
			InvertCamControl.interactable = interactable;
			ShowGrid.interactable = interactable;
			UndoButton.interactable = interactable;
			RedoButton.interactable = interactable;
			SaveButton.interactable = interactable;
			LoadButton.interactable = interactable;
		}

		private void OnDisable()
		{
			SetDefaultState(false);
		}

		public void EnableUndoButton(bool enabled)
		{
			UndoButton.interactable = enabled;
		}

		public void EnableRedoButton(bool enabled)
		{
			RedoButton.interactable = enabled;
		}

		public void EnableSaveButton(bool enabled)
		{
			SaveButton.interactable = enabled;
		}

		public void EnableLoadButton(bool enabled)
		{
			LoadButton.interactable = enabled;
		}
	}
}
