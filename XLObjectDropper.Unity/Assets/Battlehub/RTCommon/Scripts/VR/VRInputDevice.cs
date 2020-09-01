using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Battlehub.RTCommon
{
    public enum VRInputKey
    {
        Trigger
    }

    public interface IVRInputDevice
    {
        bool IsTracking
        {
            get;
        }

        Vector3 Postion
        {
            get;
        }

        Quaternion Rotation
        {
            get;
        }

        InputDevice Device
        {
            get;
        }

        bool GetKey(VRInputKey key);
        bool GetKeyDown(VRInputKey key);
        bool GetKeyUp(VRInputKey key);
    }

    [DefaultExecutionOrder(-101)]
    public class VRInputDevice : MonoBehaviour, IVRInputDevice
    {
        public bool IsTracking
        {
            get { return m_inputDevice.isValid; }
        }

        private bool m_isTriggerDown;
        private bool m_isTriggerUp;
        private bool m_isTriggerPressed;

        private Vector3 m_position;
        public Vector3 Postion
        {
            get { return m_position; }
        }

        private Quaternion m_rotation;
        public Quaternion Rotation
        {
            get { return m_rotation; }
        }

        private InputDevice m_inputDevice;
        public InputDevice Device
        {
            get { return m_inputDevice; }
            set
            {
                m_inputDevice = value;
            }
        }

        public bool GetKeyDown(VRInputKey key)
        {
            switch (key)
            {
                case VRInputKey.Trigger:
                    return m_isTriggerDown;
                default:
                    return false;
            }
        }

        public bool GetKey(VRInputKey key)
        {
            switch (key)
            {
                case VRInputKey.Trigger:
                    return m_isTriggerPressed;
                default:
                    return false;
            }
        }

        public bool GetKeyUp(VRInputKey key)
        {
            switch (key)
            {
                case VRInputKey.Trigger:
                    return m_isTriggerUp;
                default:
                    return false;
            }
        }

        private void Start()
        {
            List<InputFeatureUsage> usages = new List<InputFeatureUsage>();
            if(m_inputDevice.TryGetFeatureUsages(usages))
            {
                for(int i = 0; i < usages.Count; ++i)
                {
                    Debug.Log(usages[i].name + " " + usages[i].type);
                }
            }
        }

        private void Update()
        {
            if(m_inputDevice == null)
            {
                return;
            }

            m_isTriggerDown = false;
            m_isTriggerUp = false;
            bool isTriggerPressed;
            if (m_inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out isTriggerPressed))
            {
                if (isTriggerPressed != m_isTriggerPressed)
                {
                    m_isTriggerPressed = isTriggerPressed;
                    if(m_isTriggerPressed)
                    {
                        m_isTriggerDown = true;
                    }
                    else
                    {
                        m_isTriggerUp = true;
                    }
                }
            }

            m_inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out m_position);
            m_inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out m_rotation);
        }
    }

}

