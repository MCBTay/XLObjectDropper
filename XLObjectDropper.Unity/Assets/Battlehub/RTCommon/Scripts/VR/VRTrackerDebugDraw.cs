using UnityEngine;

namespace Battlehub.RTCommon
{
    public class VRTrackerDebugDraw : MonoBehaviour
    {
#if UNITY_2019_3_OR_NEWER
        private IVRTracker m_tracker;
        private Transform m_leftHand;
        private Transform m_rightHand;

        private void Start()
        {
            m_tracker = IOC.Resolve<IVRTracker>();
            if (m_tracker != null)
            {
                m_tracker.TrackingAquired += OnTrackingAquired;
                m_tracker.TrackingLost += OnTrackingLost;
            }
        }

        private void OnDestroy()
        {
            if (m_tracker != null)
            {
                m_tracker.TrackingAquired -= OnTrackingAquired;
                m_tracker.TrackingLost -= OnTrackingLost;
            }
        }

        private void Update()
        {
            IVRInputDevice lh = m_tracker.LeftHand;
            IVRInputDevice rh = m_tracker.RightHand;

            if (lh.IsTracking && m_leftHand != null)
            {
                m_leftHand.position = m_tracker.Origin + lh.Postion;
                m_leftHand.rotation = lh.Rotation;
            }

            if (rh.IsTracking && m_rightHand != null)
            {
                m_rightHand.position = m_tracker.Origin + rh.Postion;
                m_rightHand.rotation = rh.Rotation;
            }
        }

        private bool IsTrackedDevice(IVRInputDevice device)
        {
            return (device.Device.characteristics & UnityEngine.XR.InputDeviceCharacteristics.TrackedDevice) != 0;
        }

        private void OnTrackingAquired(IVRInputDevice device)
        {
            if(!IsTrackedDevice(device))
            {
                return;
            }

            if ((device.Device.characteristics & UnityEngine.XR.InputDeviceCharacteristics.Left) != 0)
            {
                m_leftHand = CreateModel(Color.yellow, "Left Hand").transform;
            }
            else if ((device.Device.characteristics & UnityEngine.XR.InputDeviceCharacteristics.Right) != 0)
            {
                m_rightHand = CreateModel(Color.red, "Right Hand").transform;
            }
        }

        private void OnTrackingLost(IVRInputDevice device)
        {
            if (!IsTrackedDevice(device))
            {
                return;
            }

            if ((device.Device.characteristics & UnityEngine.XR.InputDeviceCharacteristics.Left) != 0)
            {
                Destroy(m_leftHand.gameObject);
            }
            else if ((device.Device.characteristics & UnityEngine.XR.InputDeviceCharacteristics.Right) != 0)
            {
                Destroy(m_rightHand.gameObject);
            }
        }

        private GameObject CreateModel(Color color, string name)
        {
            GameObject root = new GameObject();
            root.name = name;
            root.transform.SetParent(transform, false);

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.transform.SetParent(root.transform, false);
            go.transform.localScale = Vector3.one * 0.05f;
            go.transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
            //go.transform.localPosition = Vector3.forward * -0.09f;
            go.name = "model";

            MeshRenderer renderer = go.GetComponent<MeshRenderer>();
            Material m = renderer.material;
            m.color = color;
            renderer.sharedMaterial = m;

            GameObject lineRendererGo = new GameObject("Line Renderer");
            lineRendererGo.transform.SetParent(root.transform, false);
            lineRendererGo.transform.localRotation = Quaternion.AngleAxis(45, Vector3.right);
            // lineRendererGo.transform.localPosition = Vector3.forward * -0.04f;
            LineRenderer lineRenderer = lineRendererGo.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.SetPositions(new[] { Vector3.zero, Vector3.forward * 10 });
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
            lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            lineRenderer.startWidth = 0.004f;
            lineRenderer.endWidth = 0.004f;

            return root;
        }
#endif
    }
}
