using Rewired;
using UnityEngine;
using UnityEngine.UI;

namespace XLObjectDropper.UI
{
    public class UIManager : MonoBehaviour
    {
        [HideInInspector] public Player Player { get; set; }

        [Header("Master Elements")]
        // Master Elements
        public GameObject ObjectPlacementUI;
        public GameObject ObjectSelectionUI;
        public GameObject OptionsMenuUI;


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

            //if (Application.isEditor)
            //{
            //    Player = new Player()
            //}
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
        }

        // Update is called once per frame
        private void Update()
        {
	        #region OptionsMenu
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
			#endregion

            if (Player.GetButtonDown("Start"))
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
    }
}