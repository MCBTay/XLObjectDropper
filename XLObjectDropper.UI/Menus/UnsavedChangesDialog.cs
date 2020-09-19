using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Menus
{
	public class UnsavedChangesDialog : MonoBehaviour
	{
		public GameObject YesButton;
		public GameObject NoButton;

		[HideInInspector] public event UnityAction YesClicked = () => { };
		[HideInInspector] public event UnityAction NoClicked = () => { };

		private void OnEnable()
		{
			SetDefaultState(true);
		}

		private void Start()
		{
			SetDefaultState(true);
		}

		private void OnDisable()
		{
			SetDefaultState(false);
		}

		private void SetDefaultState(bool enabled)
		{
			YesButton.GetComponent<Button>().onClick.RemoveAllListeners();
			NoButton.GetComponent<Button>().onClick.RemoveAllListeners();

			YesButton.SetActive(enabled);
			NoButton.SetActive(enabled);

			if (enabled)
			{
				YesButton.GetComponent<Button>().onClick.AddListener(delegate { YesClicked.Invoke(); });
				NoButton.GetComponent<Button>().onClick.AddListener(delegate { NoClicked.Invoke(); });
			}
		}
	}
}
