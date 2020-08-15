using System.Collections.Generic;
using System.Linq;
using GameManagement;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XLObjectDropper
{
	public class ObjectSelectionController : ListViewController<ObjectInfo>
	{
		public GameObject ObjectCategoryButtonGameObject;

        public CategoryButton ObjectCategoryButton;
        [SerializeField]
        private ObjectListItem m_itemPrefab;

        public bool showCustom { get; private set; }

        public override ListViewItem<ObjectInfo> ItemPrefab
        {
            get
            {
                return (ListViewItem<ObjectInfo>)m_itemPrefab;
            }
        }

        public override List<ObjectInfo> Items
        {
            get
            {
	            return Assets;
            }
        }

        public List<ObjectInfo> Assets { get; set; }

        private void Awake()
        {
            Debug.Log("XLObjectDropper: ObjectSelectionController.Awake()");

            Assets = new List<ObjectInfo>();
            
            m_itemPrefab = new ObjectListItem();

            scrollRect.content = new RectTransform();

            ObjectCategoryButtonGameObject = new GameObject();
            //ObjectCategoryButtonGameObject.transform.SetParent(gameObject.transform);
            ObjectCategoryButton = ObjectCategoryButtonGameObject.AddComponent<CategoryButton>();
	        ObjectCategoryButton.Label = ObjectCategoryButton.gameObject.AddComponent<TextMeshProUGUI>();
	        
            ObjectCategoryButton.OnNextCategory += ToggleShowCustom;
            ObjectCategoryButton.OnPreviousCategory += ToggleShowCustom;

            foreach (var item in AssetBundleHelper.LoadedAssets)
            {
                Assets.Add(new ObjectInfo { name = item.name });
            }
        }

        private void OnEnable()
        {
	        enabled = true;
	        gameObject.SetActive(true);

	        ObjectCategoryButtonGameObject.SetActive(true);


            UpdateList();
        }

        public void ToggleShowCustom()
        {
            showCustom = !showCustom;
            UpdateList();
            ObjectCategoryButton.Label.text = this.showCustom ? "Custom Objs" : "Official Objs";
        }

        protected override void Update()
        {
            //base.Update();
			if (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy)
			{
				if (ItemViews.Count > 0)
					EventSystem.current.SetSelectedGameObject(ItemViews.First<ListViewItem<ObjectInfo>>().gameObject);
				else
					EventSystem.current.SetSelectedGameObject(ObjectCategoryButton.gameObject);
			}

			if (!PlayerController.Instance.inputController.player.GetButtonDown("LB") && !PlayerController.Instance.inputController.player.GetButtonDown("RB"))
				return;

			ToggleShowCustom();
        }

        public override void OnItemSelected(ObjectInfo selectedObject)
        {
            // set preview object, close menu
            Debug.Log("XLObjectDropper: You selected an object, yay!");
        }
    }

	public class ObjectInfo
	{
		public string name;
	}

	public class ObjectListItem : ListViewItem<ObjectInfo>
	{
		public TMP_Text ObjectNameText;
		private ObjectInfo @object;

		public override ObjectInfo Item
		{
			get
			{
				return @object;
			}
			set
			{
				@object = value;
				ObjectNameText.text = this.@object.name;
			}
		}
	}
}
