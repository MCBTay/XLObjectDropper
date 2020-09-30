using Rewired;
using UnityEngine;
using XLObjectDropper.UI.Utilities;

namespace XLObjectDropper.UI
{
	public class UIManager : MonoBehaviour
	{
		[HideInInspector] public Player Player;
        [HideInInspector] public PlatformType PlatformType;

		[Header("Master Elements")]
        public GameObject ObjectPlacementUI;
        public GameObject ObjectSelectionUI;
        public GameObject OptionsMenuUI;
        public GameObject QuickMenuUI;
        public GameObject UnsavedChangesDialogUI;
        public GameObject ObjectEditUI;

        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Debug.Log("[XLObjectDropper.UI] UIManager.dll Initialized");
            Debug.Log("[XLObjectDropper.UI] UIManager version 0.1.7");
            
            SetDefaultState();
        }

        private void OnEnable()
        {
	        SetDefaultState();
        }

        private void SetDefaultState()
        {
	        ObjectPlacementUI.SetActive(true);
	        ObjectSelectionUI.SetActive(false);
	        OptionsMenuUI.SetActive(false);
			QuickMenuUI.SetActive(false);
			UnsavedChangesDialogUI.SetActive(false);
			ObjectEditUI.SetActive(false);
        }

        // Update is called once per frame
        private void Update()
        {
	        if (Player.GetButtonDown("B"))
	        {
		        if (OptionsMenuUI.activeInHierarchy)
		        {
			        OptionsMenuUI.SetActive(false);
			        ObjectPlacementUI.SetActive(true);
		        }

		        if (ObjectSelectionUI.activeInHierarchy)
		        {
			        ObjectSelectionUI.SetActive(false);
			        ObjectPlacementUI.SetActive(true);
		        }

		        if (QuickMenuUI.activeInHierarchy)
		        {
			        QuickMenuUI.SetActive(false);
			        ObjectPlacementUI.SetActive(true);
		        }

		        if (ObjectEditUI.activeInHierarchy)
		        {
					ObjectEditUI.SetActive(false);
					ObjectPlacementUI.SetActive(true);
		        }
			}

			
			if (Player.GetButtonDown("X") && 
				!Player.GetButton("RB") && !Player.GetButton("LB") &&
			    (ObjectPlacementUI.GetComponent<ObjectPlacementUI>().HasSelectedObject || ObjectPlacementUI.GetComponent<ObjectPlacementUI>().HasHighlightedObject))
			{
				if (!ObjectEditUI.activeInHierarchy)
				{
					ObjectPlacementUI.SetActive(false);
					ObjectEditUI.SetActive(true);
				}
				else
				{
					ObjectEditUI.SetActive(false);
					ObjectPlacementUI.SetActive(true);
				}
			}

			if (Player.GetButtonDown("Select"))
			{
				if (!OptionsMenuUI.activeInHierarchy)
				{
					ObjectPlacementUI.SetActive(false);
					OptionsMenuUI.SetActive(true);
				}
				else
				{
					OptionsMenuUI.SetActive(false);
					ObjectPlacementUI.SetActive(true);
				}
			}

	        if (Player.GetButtonTimedPressUp("Start", 0.0f, 0.7f)) // tap
			{
				if (QuickMenuUI.activeInHierarchy)
				{
					QuickMenuUI.SetActive(false);
					ObjectPlacementUI.SetActive(true);
				}
				else
				{
					if (!ObjectSelectionUI.activeInHierarchy)
					{
						ObjectPlacementUI.SetActive(false);
						ObjectSelectionUI.SetActive(true);
					}
					else
					{
						ObjectSelectionUI.SetActive(false);
						ObjectPlacementUI.SetActive(true);
					}
				}
			}
			else if (Player.GetButtonTimedPressDown("Start", 0.7f) && !ObjectSelectionUI.activeInHierarchy) //press
			{
				if (!QuickMenuUI.activeInHierarchy)
				{
					ObjectPlacementUI.SetActive(false);
					QuickMenuUI.SetActive(true);
				}
				else
				{
					QuickMenuUI.SetActive(false);
					ObjectPlacementUI.SetActive(true);
				}
			}
        }
    }
}