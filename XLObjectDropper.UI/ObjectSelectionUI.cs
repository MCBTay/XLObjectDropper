using UnityEngine;

namespace XLObjectDropper.UI
{
	public class ObjectSelectionUI : MonoBehaviour
	{
		[Header("Options Menu Elements")] 
		public GameObject MainUI;

		[Space(10)]
		public GameObject ListContent;

		[Header("Categories")] 
		public GameObject BasicCategory;
		public GameObject OtherCategory;
		public GameObject MoreCategory;
		public GameObject StuffCategory;
		public GameObject TiredCategory;
		public GameObject PacksCategory;

		[Header("UserInterface")]
		public GameObject UIButton_LB;
		public GameObject UIButton_LB_Pressed;
		public GameObject UIButton_RB;
		public GameObject UIButton_RB_Pressed;

		private void Start()
		{
			UIButton_LB.SetActive(true);
			UIButton_RB.SetActive(true);

			UIButton_LB_Pressed.SetActive(false);
			UIButton_RB_Pressed.SetActive(false);
		}

		private void Update()
		{
			#region Right bumper
			if (UIManager.Instance.Player.GetButtonDown("RB"))
			{
				UIButton_RB.SetActive(false);
				UIButton_RB_Pressed.SetActive(true);
			}
			if (UIManager.Instance.Player.GetButtonUp("RB"))
			{
				UIButton_RB.SetActive(true);
				UIButton_RB_Pressed.SetActive(false);
			}
			#endregion

			#region Left Bumper
			if (UIManager.Instance.Player.GetButtonDown("LB"))
			{
				UIButton_LB.SetActive(false);
				UIButton_LB_Pressed.SetActive(true);
			}
			if (UIManager.Instance.Player.GetButtonUp("LB"))
			{
				UIButton_LB.SetActive(true);
				UIButton_LB_Pressed.SetActive(false);
			}
			#endregion
		}
	}
}
