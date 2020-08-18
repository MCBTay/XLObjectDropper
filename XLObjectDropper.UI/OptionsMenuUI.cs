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
		public GameObject Snapping;
		public GameObject Sensitivity;
		[Space(10)]
		public GameObject UndoButton;
		public GameObject RedoButton;
		public GameObject SaveButton;
		public GameObject LoadButton;

		[HideInInspector] public event UnityAction<bool> SnappingValueChanged = (x) => { };
		[HideInInspector] public event UnityAction<float> SensitivityValueChanged = (x) => { };
		[HideInInspector] public event UnityAction UndoClicked = () => { };
		[HideInInspector] public event UnityAction RedoClicked = () => { };
		[HideInInspector] public event UnityAction SaveClicked = () => { };
		[HideInInspector] public event UnityAction LoadClicked = () => { };
		

		private void Start()
		{
			Snapping.SetActive(true);
			Sensitivity.SetActive(true);
			UndoButton.SetActive(true);
			RedoButton.SetActive(true);
			SaveButton.SetActive(true);
			LoadButton.SetActive(true);

			Snapping.GetComponent<Toggle>().onValueChanged.AddListener(delegate { SnappingValueChanged.Invoke(Snapping.GetComponent<Toggle>().isOn); });
			Sensitivity.GetComponent<Slider>().onValueChanged.AddListener(delegate { SensitivityValueChanged.Invoke(Sensitivity.GetComponent<Slider>().value); });
			UndoButton.GetComponent<Button>().onClick.AddListener(delegate { UndoClicked.Invoke(); });
			RedoButton.GetComponent<Button>().onClick.AddListener(delegate { RedoClicked.Invoke(); });
			SaveButton.GetComponent<Button>().onClick.AddListener(delegate { SaveClicked.Invoke(); });
			LoadButton.GetComponent<Button>().onClick.AddListener(delegate { LoadClicked.Invoke(); });
		}

		private void Update()
		{

		}
	}
}
