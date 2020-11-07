/// Credit zero3growlithe
/// sourced from: http://forum.unity3d.com/threads/scripts-useful-4-6-scripts-collection.264161/page-2#post-2011648

/*USAGE:
Simply place the script on the ScrollRect that contains the selectable children we'll be scrolling to
and drag'n'drop the RectTransform of the options "container" that we'll be scrolling.*/

using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI.Extensions
{
	[RequireComponent(typeof(ScrollRect))]
    [AddComponentMenu("UI/Extensions/UIScrollToSelection")]
    public class UIScrollToSelection : MonoBehaviour
    {

        //*** ATTRIBUTES ***//
        [Header("[ Settings ]")]
        [SerializeField]
        private ScrollType scrollDirection = ScrollType.BOTH;
        [SerializeField]
        private float scrollSpeed = 10f;

        [Header("[ Input ]")]
        [SerializeField]
        private bool cancelScrollOnInput = false;
        [SerializeField]
        private List<KeyCode> cancelScrollKeycodes = new List<KeyCode>();

        //*** PROPERTIES ***//
        // REFERENCES
        protected RectTransform LayoutListGroup => TargetScrollRect != null ? TargetScrollRect.content : null;

        // SETTINGS
        protected ScrollType ScrollDirection => scrollDirection;
        protected float ScrollSpeed => scrollSpeed;

        // INPUT
        protected bool CancelScrollOnInput => cancelScrollOnInput;
        protected List<KeyCode> CancelScrollKeycodes => cancelScrollKeycodes;

        // CACHED REFERENCES
        protected RectTransform ScrollWindow { get; set; }
        protected ScrollRect TargetScrollRect { get; set; }

        // SCROLLING
        protected EventSystem CurrentEventSystem => EventSystem.current;
        protected GameObject LastCheckedGameObject { get; set; }
        protected GameObject CurrentSelectedGameObject => EventSystem.current.currentSelectedGameObject;
        protected RectTransform CurrentTargetRectTransform { get; set; }
        protected bool IsManualScrollingAvailable { get; set; }

        //*** METHODS - PUBLIC ***//


        //*** METHODS - PROTECTED ***//
        protected virtual void Awake()
        {
            TargetScrollRect = GetComponent<ScrollRect>();
            ScrollWindow = TargetScrollRect.GetComponent<RectTransform>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            UpdateReferences();
            CheckIfScrollingShouldBeLocked();
            ScrollRectToLevelSelection();
        }

        //*** METHODS - PRIVATE ***//
        private void UpdateReferences()
        {
            // update current selected rect transform
            if (CurrentSelectedGameObject != LastCheckedGameObject)
            {
                CurrentTargetRectTransform = (CurrentSelectedGameObject != null) ?
                    CurrentSelectedGameObject.GetComponent<RectTransform>() :
                    null;

                // unlock automatic scrolling
                if (CurrentSelectedGameObject != null &&
                    CurrentSelectedGameObject.transform.parent == LayoutListGroup.transform)
                {
                    IsManualScrollingAvailable = false;
                }
            }

            LastCheckedGameObject = CurrentSelectedGameObject;
        }

        private void CheckIfScrollingShouldBeLocked()
        {
	        if (!CancelScrollOnInput || IsManualScrollingAvailable) return;

	        foreach (var t in CancelScrollKeycodes)
	        {
		        if (Input.GetKeyDown(t))
		        {
			        IsManualScrollingAvailable = true;
			        break;
		        }
	        }
        }

        //private bool objectEdit = false;

        private void ScrollRectToLevelSelection()
        {
            // check main references
            bool referencesAreIncorrect = (TargetScrollRect == null || LayoutListGroup == null || ScrollWindow == null);

            if (referencesAreIncorrect || IsManualScrollingAvailable) return;

            RectTransform selection = CurrentTargetRectTransform;

            // check if scrolling is possible
            if (selection == null || selection.transform.parent != LayoutListGroup.transform)
            {
	            // XLOD: customized for Object Edit where selectables aren't direct children
                //if (selection.transform.parent?.parent?.parent != LayoutListGroup.transform && selection.transform.parent?.parent?.parent?.parent != LayoutListGroup.transform)
	            //{
		           // objectEdit = true;
	            //}

	            return;
            }

            // depending on selected scroll direction move the scroll rect to selection
            switch (ScrollDirection)
            {
                case ScrollType.VERTICAL:
                    UpdateVerticalScrollPosition(selection);
                    break;
                case ScrollType.HORIZONTAL:
                    UpdateHorizontalScrollPosition(selection);
                    break;
                case ScrollType.BOTH:
                    UpdateVerticalScrollPosition(selection);
                    UpdateHorizontalScrollPosition(selection);
                    break;
            }
        }

        private void UpdateVerticalScrollPosition(RectTransform selection)
        {
            // move the current scroll rect to correct position
            float selectionPosition = -selection.anchoredPosition.y;// - (selection.rect.height * (1 - selection.pivot.y));

            float elementHeight = selection.rect.height;
            float maskHeight = ScrollWindow.rect.height;
            float listAnchorPosition = LayoutListGroup.anchoredPosition.y;

            // get the element offset value depending on the cursor move direction
            float offlimitsValue = GetScrollOffset(selectionPosition, listAnchorPosition, elementHeight, maskHeight);

            // move the target scroll rect
            TargetScrollRect.verticalNormalizedPosition +=
                (offlimitsValue / LayoutListGroup.rect.height) * Time.unscaledDeltaTime * scrollSpeed;
        }

        private void UpdateHorizontalScrollPosition(RectTransform selection)
        {
            // move the current scroll rect to correct position
            float selectionPosition = -selection.anchoredPosition.x - (selection.rect.width * (1 - selection.pivot.x));

            float elementWidth = selection.rect.width;
            float maskWidth = ScrollWindow.rect.width;
            float listAnchorPosition = -LayoutListGroup.anchoredPosition.x;

            // get the element offset value depending on the cursor move direction
            float offlimitsValue = -GetScrollOffset(selectionPosition, listAnchorPosition, elementWidth, maskWidth);

            // move the target scroll rect
            TargetScrollRect.horizontalNormalizedPosition +=
                (offlimitsValue / LayoutListGroup.rect.width) * Time.unscaledDeltaTime * scrollSpeed;
        }

        private float GetScrollOffset(float position, float listAnchorPosition, float targetLength, float maskLength)
        {
	        if (position < listAnchorPosition + (targetLength / 2))
            {
	            return (listAnchorPosition + maskLength) - (position - targetLength);
            }
            else if (position + targetLength > listAnchorPosition + maskLength)
            {
	            return (listAnchorPosition + maskLength) - (position + targetLength);
            }

            return 0;
        }

        //*** ENUMS ***//
        public enum ScrollType
        {
            VERTICAL,
            HORIZONTAL,
            BOTH
        }
    }
}