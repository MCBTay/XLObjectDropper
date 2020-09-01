using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.EventSystems
{
    /// <summary>
    /// Remove this class if real OVRInput.cs exist
    /// </summary>
    public class OVRInput : MonoBehaviour
    {
        public enum Button
        {
            One
        }

        public enum Axis2D
        {
            SecondaryThumbstick
        }

        public static bool GetUp(Button button)
        {
            return false;
        }

        public static bool GetDown(Button button)
        {
            return false;
        }

        public static Vector2 Get(Axis2D axis)
        {
            return Vector2.zero;
        }
    }
}
