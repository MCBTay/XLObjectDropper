using UnityEngine;
using UnityEngine.EventSystems;

namespace Battlehub.RTCommon
{
    public class RTEVRInputModule : OVRInputModule
    {
        public IRTE Editor;

        protected override void Start()
        {
            base.Start();
        }

        protected override PointerEventData.FramePressState GetGazeButtonState()
        {
            var pressed = Editor.Input.GetPointerDown(0);
            var release = Editor.Input.GetPointerUp(0);

            if (pressed && release)
                return PointerEventData.FramePressState.PressedAndReleased;
            if (pressed)
                return PointerEventData.FramePressState.Pressed;
            if (release)
                return PointerEventData.FramePressState.Released;
            return PointerEventData.FramePressState.NotChanged;
        }
    }
}

