using UnityEngine;

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

		private void Start()
		{
			Snapping.SetActive(true);
			Sensitivity.SetActive(true);
			UndoButton.SetActive(true);
			RedoButton.SetActive(true);
			SaveButton.SetActive(true);
			LoadButton.SetActive(true);
		}

		private void Update()
		{
			// handle button clicks and value changes here
		}
	}
}
