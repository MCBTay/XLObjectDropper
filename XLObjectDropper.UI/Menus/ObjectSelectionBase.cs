using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLObjectDropper.UI.Controls;
using XLObjectDropper.UI.Controls.ControllerButtons;
using XLObjectDropper.UI.Controls.ListItems;

namespace XLObjectDropper.UI.Menus
{
	public class ObjectSelectionBase<T> : MonoBehaviour where T : Enum
	{
		public GameObject MainUI;
		public GameObject ListContent;
		public GameObject ListItemPrefab;

		public BumperController Bumpers;

		[HideInInspector] public Dictionary<T, GameObject> Categories;
		[HideInInspector] public int CurrentCategoryIndex;
		[HideInInspector] public UnityAction CategoryChanged = () => { };

		public Animator Animator;

		protected virtual void Awake()
		{
			Categories = new Dictionary<T, GameObject>();
			CurrentCategoryIndex = 0;
		}

		protected void OnEnable()
		{
			Animator.Play("SlideIn");
		}

		void OnDisable()
		{
		}

		protected void Start()
		{
			CurrentCategoryIndex = -1;
			SetActiveCategory(true);
		}

		private void Update()
		{
			Bumpers.RightBumper.EnableButton(true);
			Bumpers.LeftBumper.EnableButton(true);

			if (UIManager.Instance.Player.GetButtonDown("RB"))
			{
				SetActiveCategory(true);
			}

			if (UIManager.Instance.Player.GetButtonDown("LB"))
			{
				SetActiveCategory(false);
			}
		}

		public void SetActiveCategory(bool increment)
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
				if (category.Key.Equals((T)Enum.Parse(typeof(T), CurrentCategoryIndex.ToString(), true)))
				{
					category.Value.GetComponent<Image>().color = new Color(0.196078f, 0.525490f, 0.925490f, 1.0f);
				}
				else
				{
					category.Value.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.392156f);
				}
			}

			CategoryChanged.Invoke();

			if (ListContent.transform.childCount > 0)
				EventSystem.current.SetSelectedGameObject(ListContent.transform.GetChild(0).gameObject);
		}

		public void ClearList()
		{
			for (var i = ListContent.transform.childCount - 1; i >= 0; i--)
			{
				var objectA = ListContent.transform.GetChild(i);
				objectA.transform.parent = null;

				objectA.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
				objectA.gameObject.GetComponent<ObjectSelectionListItem>().onSelect.RemoveAllListeners();
				
				Destroy(objectA.gameObject);
			}

			EventSystem.current.SetSelectedGameObject(null);
		}

		public GameObject AddToList(string name, Texture2D previewTexture, UnityAction objectClicked = null, UnityAction objectSelected = null)
		{
			var listItem = Instantiate(ListItemPrefab, ListContent.transform);
			
			listItem.GetComponentInChildren<TMP_Text>().SetText(name.Replace('_', ' '));

			if (objectClicked != null)
			{
				listItem.GetComponent<Button>().onClick.AddListener(objectClicked);
			}

			if (objectSelected != null)
			{
				listItem.GetComponent<ObjectSelectionListItem>().onSelect.AddListener(objectSelected);
			}
			
			listItem.GetComponentInChildren<RawImage>().texture = previewTexture;

			listItem.SetActive(true);

			return listItem;
		}
	}
}
