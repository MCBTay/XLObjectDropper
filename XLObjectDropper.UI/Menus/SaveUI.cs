using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Menus
{
	public class SaveUI : MonoBehaviour
	{
		public Button SaveButton;
		public Button CancelButton;
		public TMP_InputField InputField;
		public Animator Animator;

		private void OnEnable()
		{
			Animator.Play("SlideIn");
			InputField.input
		}
	}
}
