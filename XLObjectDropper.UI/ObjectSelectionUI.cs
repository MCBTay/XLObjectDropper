using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XLObjectDropper.UI
{
	public class ObjectSelectionUI : MonoBehaviour
	{
		[Header("Options Menu Elements")] 
		public GameObject MainUI;

		[Header("ListContent")] 
		public GameObject ListContent;

		[Header("Categories")]
		public GameObject RailsCategory;
		public GameObject RampsCategory;
		public GameObject SplinesCategory;
		public GameObject PropsCategory;
		public GameObject ParkCategory;
		public GameObject PacksCategory;

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
			Categories.Add(SpawnableType.Rails, RailsCategory);
			Categories.Add(SpawnableType.Ramps, RampsCategory);
			Categories.Add(SpawnableType.Splines, SplinesCategory);
			Categories.Add(SpawnableType.Props, PropsCategory);
			Categories.Add(SpawnableType.Park, ParkCategory);
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
				if (category.Key == (SpawnableType)CurrentCategoryIndex)
				{
					category.Value.GetComponent<Image>().color = new Color(0.196078f, 0.525490f, 0.925490f, 1.0f);
				}
				else
				{
					category.Value.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.392156f);
				}
			}

			if (ListContent.transform.childCount > 0)
				EventSystem.current.SetSelectedGameObject(ListContent.transform.GetChild(0).gameObject);
		}
	}
}
