using UnityEngine;

namespace UnityEngine.EventSystems
{
    /// <summary>
    /// Remove this class if real OVRCameraRig.cs exist
    /// </summary>
    public class OVRCameraRig : MonoBehaviour
    {
        public Camera leftEyeCamera
        {
            get { return Camera.main; }
        }
    }
}
