using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XLObjectDropper.UI
{
	public class ObjectSelectionUI : MonoBehaviour
	{
		[Header("Options Menu Elements")] 
		public GameObject MainUI;

		[Header("Categories")] 
		public GameObject BasicCategory;
		public GameObject OtherCategory;
		public GameObject MoreCategory;
		public GameObject StuffCategory;
		public GameObject TiredCategory;
		public GameObject PacksCategory;

		[Header("Categories List Content")]
		public GameObject BasicCategoryListContent;
		public GameObject OtherCategoryListContent;
		public GameObject MoreCategoryListContent;
		public GameObject StuffCategoryListContent;
		public GameObject TiredCategoryListContent;
		public GameObject PacksCategoryListContent;

		[HideInInspector]
		public Dictionary<SpawnableType, GameObject> Categories;
		[HideInInspector]
		private int CurrentCategoryIndex;

		[Header("UserInterface")]
		public GameObject UIButton_LB;
		public GameObject UIButton_LB_Pressed;
		public GameObject UIButton_RB;
		public GameObject UIButton_RB_Pressed;

		private void Awake()
		{
			Categories = new Dictionary<SpawnableType, GameObject>();

			Categories.Add(SpawnableType.Basic, BasicCategory);
			Categories.Add(SpawnableType.Other, OtherCategory);
			Categories.Add(SpawnableType.More, MoreCategory);
			Categories.Add(SpawnableType.Stuff, StuffCategory);
			Categories.Add(SpawnableType.Tired, TiredCategory);
			Categories.Add(SpawnableType.Packs, PacksCategory);

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
				category.Value.SetActive(false);
			}

			var activeCategory = Categories.ElementAt(CurrentCategoryIndex);
			activeCategory.Value.SetActive(true);

			var activeList = GetListByType(activeCategory.Key);
			if (activeList.transform.childCount > 0)
				EventSystem.current.SetSelectedGameObject(activeList.transform.GetChild(0).gameObject);
		}

		public GameObject GetListByType(SpawnableType type)
		{
			switch (type)
			{
				case SpawnableType.Other: return OtherCategoryListContent;
				case SpawnableType.More: return MoreCategoryListContent;
				case SpawnableType.Stuff: return StuffCategoryListContent;
				case SpawnableType.Tired: return TiredCategoryListContent;
				case SpawnableType.Packs: return PacksCategoryListContent;
				case SpawnableType.Basic:
				default:
					return BasicCategoryListContent;
			}
		}
	}
}
