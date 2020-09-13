using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace XLObjectDropper.UI
{
	public class OptionsMenuUI : MonoBehaviour
	{
		[Header("Options Menu Elements")]
		public GameObject MainUI;
		[Space(10)]
		public GameObject Sensitivity;
		public GameObject InvertCamControl;
		public GameObject ShowGrid;

		[Space(10)]
		public GameObject UndoButton;
		public GameObject RedoButton;
		public GameObject SaveButton;
		public GameObject LoadButton;

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
		}

		private void Start()
		{
			SetDefaultState(true);
		}

		private void SetDefaultState(bool enabled)
		{
			Sensitivity.GetComponent<Slider>().onValueChanged.RemoveAllListeners();
			InvertCamControl.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
			ShowGrid.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
			UndoButton.GetComponent<Button>().onClick.RemoveAllListeners();
			RedoButton.GetComponent<Button>().onClick.RemoveAllListeners();
			SaveButton.GetComponent<Button>().onClick.RemoveAllListeners();
			LoadButton.GetComponent<Button>().onClick.RemoveAllListeners();

			Sensitivity.SetActive(enabled);
			InvertCamControl.SetActive(enabled);
			ShowGrid.SetActive(enabled);
			UndoButton.SetActive(enabled);
			RedoButton.SetActive(enabled);
			SaveButton.SetActive(enabled);
			LoadButton.SetActive(enabled);

			if (enabled)
			{
				Sensitivity.GetComponent<Slider>().onValueChanged.AddListener(delegate { SensitivityValueChanged.Invoke(Sensitivity.GetComponent<Slider>().value); });
				InvertCamControl.GetComponent<Toggle>().onValueChanged.AddListener(delegate { InvertCamControlValueChanged.Invoke(InvertCamControl.GetComponent<Toggle>().isOn); });
				ShowGrid.GetComponent<Toggle>().onValueChanged.AddListener(delegate { ShowGridValueChanged.Invoke(ShowGrid.GetComponent<Toggle>().isOn); });
				UndoButton.GetComponent<Button>().onClick.AddListener(delegate { UndoClicked.Invoke(); });
				RedoButton.GetComponent<Button>().onClick.AddListener(delegate { RedoClicked.Invoke(); });
				SaveButton.GetComponent<Button>().onClick.AddListener(delegate { SaveClicked.Invoke(); });
				LoadButton.GetComponent<Button>().onClick.AddListener(delegate { LoadClicked.Invoke(); });
			}
		}

		private void OnDisable()
		{
			SetDefaultState(false);
		}

		public void EnableUndoButton(bool enabled)
		{
			UndoButton.GetComponent<Button>().interactable = enabled;
		}

		public void EnableRedoButton(bool enabled)
		{
			RedoButton.GetComponent<Button>().interactable = enabled;
		}

		public void EnableSaveButton(bool enabled)
		{
			SaveButton.GetComponent<Button>().interactable = enabled;
		}

		public void EnableLoadButton(bool enabled)
		{
			LoadButton.GetComponent<Button>().interactable = enabled;
		}
	}
}
