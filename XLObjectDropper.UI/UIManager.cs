﻿using Rewired;
using System.Collections;
using UnityEngine;
using XLObjectDropper.UI.Menus;
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
        private bool ObjectSelectionActive => ObjectSelectionUI != null && ObjectSelectionUI.activeInHierarchy;

		public GameObject OptionsMenuUI;
		private bool OptionsMenuActive => OptionsMenuUI != null && OptionsMenuUI.activeInHierarchy;

		public GameObject QuickMenuUI;
		private bool QuickMenuActive => QuickMenuUI != null && QuickMenuUI.activeInHierarchy;

		public GameObject UnsavedChangesDialogUI;

        public GameObject ObjectEditUI;
        private bool ObjectEditActive => ObjectEditUI != null && ObjectEditUI.activeInHierarchy;

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
		        if (OptionsMenuUI.activeInHierarchy && 
		            !OptionsMenuUI.GetComponent<OptionsMenuUI>().LoadSavedActive && 
		            !OptionsMenuUI.GetComponent<OptionsMenuUI>().SaveActive)
			        StartCoroutine(DisableOptionsMenu());

		        if (ObjectSelectionUI.activeInHierarchy) 
			        StartCoroutine(DisableSelection());

		        if (QuickMenuUI.activeInHierarchy) 
			        StartCoroutine(DisableQuickMenu());

		        if (ObjectEditUI.activeInHierarchy) 
			        StartCoroutine(DisableObjectEdit());
			}

			//TODO: Get rid of this shit, this should be a child of object movement
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
					StartCoroutine(DisableObjectEdit());
				}
			}

			if (Player.GetButtonDown("Select") && !ObjectEditActive && !ObjectSelectionActive && !QuickMenuActive)
			{
				if (!OptionsMenuUI.activeInHierarchy)
				{
					ObjectPlacementUI.SetActive(false);
					OptionsMenuUI.SetActive(true);
				}
				else
				{
					StartCoroutine(DisableOptionsMenu());
				}
			}

	        if (Player.GetButtonTimedPressUp("Start", 0.0f, 0.7f) && !ObjectEditActive && !OptionsMenuActive) // tap
			{
				if (QuickMenuUI.activeInHierarchy)
				{
					StartCoroutine(DisableQuickMenu());
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
						StartCoroutine(DisableSelection());
					}
				}
			}
			else if (Player.GetButtonTimedPressDown("Start", 0.7f) && !ObjectSelectionActive && !OptionsMenuActive && !ObjectEditActive) //press
			{
				if (!QuickMenuUI.activeInHierarchy)
				{
					ObjectPlacementUI.SetActive(false);
					QuickMenuUI.SetActive(true);
				}
				else
				{
					StartCoroutine(DisableQuickMenu());
				}
			}
        }

		public IEnumerator DisableSelection()
		{
			ObjectSelectionUI.GetComponent<ObjectSelectionUI>().Animator.Play("SlideOut");

			yield return new WaitForSeconds(0.2f);

			ObjectSelectionUI.SetActive(false);
			ObjectPlacementUI.SetActive(true);
		}

		public IEnumerator DisableQuickMenu()
		{
			QuickMenuUI.GetComponent<QuickMenuUI>().Animator.Play("SlideOut");

			yield return new WaitForSeconds(0.2f);

			QuickMenuUI.SetActive(false);
			ObjectPlacementUI.SetActive(true);
		}

		public IEnumerator DisableOptionsMenu()
		{
			var optionsMenu = OptionsMenuUI.GetComponent<OptionsMenuUI>();
			if (optionsMenu == null) yield break;

			if (optionsMenu.LoadSavedActive)
			{
				var loadSavedUI = optionsMenu.LoadSavedUI.GetComponent<LoadSavedUI>();

				if (loadSavedUI != null)
				{
					if (loadSavedUI.UnsavedChangesDialog != null && loadSavedUI.UnsavedChangesDialog.activeInHierarchy)
					{
						loadSavedUI.DestroyUnsavedChangesDialog();
					}

					StartCoroutine(optionsMenu.DisableLoadSavedUI());
				}
			}

			if (optionsMenu.SaveActive)
			{
				var saveUI = optionsMenu.SaveUI.GetComponent<SaveUI>();

				if (saveUI != null && saveUI.gameObject.activeInHierarchy)
				{
					StartCoroutine(optionsMenu.DisableSaveUI());
				}
			}

			optionsMenu?.Animator.Play("SlideOut");

			yield return new WaitForSeconds(0.2f);

			OptionsMenuUI.SetActive(false);
			ObjectPlacementUI.SetActive(true);
		}

		public IEnumerator DisableObjectEdit()
		{
			ObjectEditUI.GetComponent<ObjectEditUI>().Animator.Play("SlideOut");

			yield return new WaitForSeconds(0.2f);

			ObjectEditUI.SetActive(false);
			ObjectPlacementUI.SetActive(true);
		}
	}
}