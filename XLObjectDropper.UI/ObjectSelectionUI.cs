using System.Collections.Generic;
using System.Linq;
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

		[HideInInspector]
		public List<GameObject> Categories;
		[HideInInspector]
		private int CurrentCategoryIndex;

		[Header("UserInterface")]
		public GameObject UIButton_LB;
		public GameObject UIButton_LB_Pressed;
		public GameObject UIButton_RB;
		public GameObject UIButton_RB_Pressed;

		private void Awake()
		{
			Categories = new List<GameObject>();

			Categories.Add(BasicCategory);
			Categories.Add(OtherCategory);
			Categories.Add(MoreCategory);
			Categories.Add(StuffCategory);
			Categories.Add(TiredCategory);
			Categories.Add(PacksCategory);

			CurrentCategoryIndex = 0;
		}

		private void Start()
		{
			CurrentCategoryIndex = -1;
			SetActiveCategory(true);

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

				SetActiveCategory(true);
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

				SetActiveCategory(false);
			}
			if (UIManager.Instance.Player.GetButtonUp("LB"))
			{
				UIButton_LB.SetActive(true);
				UIButton_LB_Pressed.SetActive(false);
			}
			#endregion
		}

		private void SetActiveCategory(bool increment)
		{
			if (increment) CurrentCategoryIndex++;
			else CurrentCategoryIndex--;

			if (CurrentCategoryIndex > Categories.Count - 1)
			{
				CurrentCategoryIndex = 0;
			}

			if (CurrentCategoryIndex < 0)
			{
				CurrentCategoryIndex = Categories.Count - 1;
			}

			foreach (var category in Categories)
			{
				category.SetActive(false);
			}

			Categories.ElementAt(CurrentCategoryIndex).SetActive(true);
		}
	}
}
