using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using XLObjectDropper.UI.Controls;

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

		private void SetDefaultState(bool enabled)
		{
			Sensitivity.onValueChanged.RemoveAllListeners();
			InvertCamControl.onValueChanged.RemoveAllListeners();
			ShowGrid.onValueChanged.RemoveAllListeners();
			UndoButton.onClick.RemoveAllListeners();
			RedoButton.onClick.RemoveAllListeners();
			SaveButton.onClick.RemoveAllListeners();
			LoadButton.onClick.RemoveAllListeners();

			Sensitivity.gameObject.SetActive(enabled);
			InvertCamControl.gameObject.SetActive(enabled);
			ShowGrid.gameObject.SetActive(enabled);
			UndoButton.gameObject.SetActive(enabled);
			RedoButton.gameObject.SetActive(enabled);
			SaveButton.gameObject.SetActive(enabled);
			LoadButton.gameObject.SetActive(enabled);

			if (enabled)
			{
				Sensitivity.onValueChanged.AddListener(delegate { SensitivityValueChanged.Invoke(Sensitivity.GetComponent<Slider>().value); });
				InvertCamControl.onValueChanged.AddListener(delegate { InvertCamControlValueChanged.Invoke(InvertCamControl.GetComponent<Toggle>().isOn); });
				ShowGrid.onValueChanged.AddListener(delegate { ShowGridValueChanged.Invoke(ShowGrid.GetComponent<Toggle>().isOn); });
				UndoButton.onClick.AddListener(delegate { UndoClicked.Invoke(); });
				RedoButton.onClick.AddListener(delegate { RedoClicked.Invoke(); });
				SaveButton.onClick.AddListener(delegate { SaveClicked.Invoke(); });
				LoadButton.onClick.AddListener(delegate { LoadClicked.Invoke(); });
			}
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
