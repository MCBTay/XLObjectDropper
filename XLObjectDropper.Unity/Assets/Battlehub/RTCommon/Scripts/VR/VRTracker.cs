using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Battlehub.RTCommon
{
    public interface IVRTracker
    {
        event Action<IVRInputDevice> TrackingAquired;
        event Action<IVRInputDevice> TrackingLost;

        IVRInputDevice LeftHand
        {
            get;
        }

        IVRInputDevice RightHand
        {
            get;
        }

        Vector3 Origin
        {
            get;
        }
    }

    public class VRTracker : RTEComponent, IVRTracker
    {
        public event Action<IVRInputDevice> TrackingAquired;
        public event Action<IVRInputDevice> TrackingLost;

        private List<XRNodeState> nodeStates = new List<XRNodeState>();
        private Vector3 m_origin;
        public Vector3 Origin
        {
            get { return m_origin; }
        }

        private VRInputDevice m_nullDevice;
        private VRInputDevice m_leftHand;
        public IVRInputDevice LeftHand
        {
            get { return m_leftHand; }
        }

        private VRInputDevice m_rightHand;
        public IVRInputDevice RightHand
        {
            get { return m_rightHand; }
        }

        protected override void AwakeOverride()
        {
            base.AwakeOverride();
            m_nullDevice = gameObject.AddComponent<VRInputDevice>();
            m_nullDevice.enabled = false;

            m_leftHand = m_nullDevice;
            m_rightHand = m_nullDevice;

            IOC.RegisterFallback<IVRTracker>(this);

            InputTracking.trackingAcquired += OnTrackingAquired;
            InputTracking.trackingLost += OnTrackingLost;
        }

        protected override void OnDestroyOverride()
        {
            base.OnDestroyOverride();
        
            InputTracking.trackingAcquired -= OnTrackingAquired;
            InputTracking.trackingLost -= OnTrackingLost;

            IOC.UnregisterFallback<IVRTracker>(this);

            if(m_leftHand != m_nullDevice)
            {
                OnTrackingLost(XRNode.LeftHand);
            }

            if(m_rightHand != m_nullDevice)
            {
                OnTrackingLost(XRNode.RightHand);
            }
        }

        private void OnTrackingAquired(XRNodeState state)
        {
            Debug.Log("Tracking Aquired " + state.nodeType);

            if (state.nodeType == XRNode.LeftHand)
            {
                if(m_leftHand == m_nullDevice)
                {
                    m_leftHand = CreateVRInputDevice(gameObject, state);
                    RaiseTrackingAcquired(m_leftHand);
                }
            }
            else if (state.nodeType == XRNode.RightHand)
            {
                if(m_rightHand == m_nullDevice)
                {
                    m_rightHand = CreateVRInputDevice(gameObject, state);
                    RaiseTrackingAcquired(m_rightHand);
                }   
            }
        }

        private void OnTrackingLost(XRNodeState state)
        {
            Debug.Log("Tracking Lost " + state.nodeType);
            OnTrackingLost(state.nodeType);
        }

        private void OnTrackingLost(XRNode nodeType)
        {
            if (nodeType == XRNode.LeftHand)
            {
                if(m_leftHand != m_nullDevice)
                {
                    RaiseTrackingLost(m_leftHand);
                    Destroy(m_leftHand);
                    m_leftHand = m_nullDevice;
                }
            }
            else if (nodeType == XRNode.RightHand)
            {
                if(m_rightHand != m_nullDevice)
                {
                    RaiseTrackingLost(m_rightHand);
                    Destroy(m_rightHand);
                    m_rightHand = m_nullDevice;
                }
            }
        }

        private static VRInputDevice CreateVRInputDevice(GameObject go, XRNodeState state)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(state.nodeType);
            if (device != null)
            {
                VRInputDevice vrInputDevice = go.AddComponent<VRInputDevice>();
                vrInputDevice.Device = device;
                return vrInputDevice;
            }

            return null;
        }

        private void Update()
        {
            nodeStates.Clear();

            InputTracking.GetNodeStates(nodeStates);
            foreach (XRNodeState state in nodeStates)
            {
                Vector3 pos;
                if (state.TryGetPosition(out pos))
                {
                    if (state.nodeType == XRNode.Head)
                    {
                        m_origin = Window.Camera.transform.position - pos;
                    }
                    if (state.nodeType == XRNode.LeftHand)
                    {
                        if(m_leftHand == m_nullDevice)
                        {
                            OnTrackingAquired(state);
                        }
                    }
                    else if (state.nodeType == XRNode.RightHand)
                    {
                        if (m_rightHand == m_nullDevice)
                        {
                            OnTrackingAquired(state);
                        }
                    }
                }
            }
        }

        private void RaiseTrackingAcquired(IVRInputDevice device)
        {
            if(device == null || ((object)device) == m_nullDevice)
            {
                return;
            }

            if(TrackingAquired != null)
            {
                TrackingAquired(device);
            }
        }

        private void RaiseTrackingLost(IVRInputDevice device)
        {
            if(device == null || ((object)device) == m_nullDevice)
            {
                return;
            }

            if(TrackingLost != null)
            {
                TrackingLost(device);
            }
        }
    }
}


