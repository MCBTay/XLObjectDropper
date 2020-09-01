using Battlehub.RTCommon;
using UnityEngine;
using UnityEngine.XR;

namespace Battlehub.RTHandles
{
    public class RuntimeSceneVRComponent : RuntimeSelectionComponent
    {
        private enum VRTool
        {
            None,
            Recenter,
            MoveXZ,
            MoveY,
            PhysicsRaycast
        }

        private VRTool m_vrTool;
        private VRTool VrTool
        {
            get { return m_vrTool; }
            set
            {
                if(m_vrTool != value)
                {
                    m_vrTool = value;
                    OnVRToolChanged();
                }
            }
        }

        [SerializeField]
        private float m_height = 1.76f;
        [SerializeField]
        private Transform m_cameraRig = null;
        [SerializeField]
        private VRRecenterTool m_recenterTool = null;
        [SerializeField]
        private VRMoveXZTool m_moveXZTool = null;
        [SerializeField]
        private VRMoveYTool m_moveYTool = null;
        [SerializeField]
        private VRPhysicsRaycastTool m_physicsRaycastTool = null;

        [SerializeField]
        private RTEVRGazePointer m_gazePointer = null;
        public RTEVRGazePointer GazePointer
        {
            get { return m_gazePointer; }
            set { m_gazePointer = value; }
        }

        private float m_progress;
        public float Progress
        {
            get { return m_progress; }
            set
            {
                m_progress = value;
                if(m_gazePointer != null)
                {
                    m_gazePointer.Progress = 1 - value;
                }
            }
        }

        public void BeginShowProgress()
        {
            m_gazePointer.gameObject.SetActive(true);
        }

        public void EndShowProgress()
        {
            UpdateGazePointerVisibility();
        }

        private Quaternion m_targetRotation = Quaternion.identity;

        public void Lock()
        {
            Progress = 0;
            VrTool = VRTool.None;
        }

        public void BeginRecenter()
        {
            Editor.Tools.Current = RuntimeTool.View;
            VrTool = VRTool.Recenter;
        }

        public void BeginMoveXY()
        {
            Editor.Tools.Current = RuntimeTool.View;
            VrTool = VRTool.MoveXZ;
        }

        public void BeginMoveZ()
        {
            Editor.Tools.Current = RuntimeTool.View;
            VrTool = VRTool.MoveY;
        }

        public void BeginCurvedRaycast()
        {
            Editor.Tools.Current = RuntimeTool.View;
            VrTool = VRTool.PhysicsRaycast;
        }

        public void SwitchToPositionHandle()
        {
            Editor.Tools.Current = RuntimeTool.Move;
            VrTool = VRTool.None;
        }

        public void SwitchToRotationHandle()
        {
            Editor.Tools.Current = RuntimeTool.Rotate;
            VrTool = VRTool.None;
        }

        public void SwitchToScaleHandle()
        {
            Editor.Tools.Current = RuntimeTool.Scale;
            VrTool = VRTool.None;
        }

        public void Unlock()
        {
            Progress = 0;
        }

        public void Action()
        {
            switch (VrTool)
            {
                case VRTool.PhysicsRaycast:
                    {
                        if(m_physicsRaycastTool != null)
                        {
                            Vector3 position = m_physicsRaycastTool.transform.position;
                            position.y += m_height;
                            m_cameraRig.position = position;
                        }
                    }
                    break;
                case VRTool.MoveXZ:
                    {
                        if(m_moveXZTool != null)
                        {
                            Vector3 xz = m_moveXZTool.transform.position;
                            xz.y = m_cameraRig.position.y;
                            m_cameraRig.position = xz;
                        }
                    }
                    break;
                case VRTool.MoveY:
                    {
                        if(m_moveYTool != null)
                        {
                            Vector3 y = m_cameraRig.position;
                            y.y = m_moveYTool.transform.position.y;
                            m_cameraRig.position = y;
                        }
                    }
                    break;
                case VRTool.Recenter:
                    {
                        if (m_recenterUsingButton)
                        {
                            m_recenterUsingButton = false;
                        }
                        else
                        {
                            m_targetRotation = Quaternion.LookRotation(Window.Pointer.Ray.direction);
                        }

                    }
                    break;
            }
        }

        private void OnVRToolChanged()
        {
            if (m_physicsRaycastTool != null)
            {
                m_physicsRaycastTool.Component = this;
                m_physicsRaycastTool.gameObject.SetActive(VrTool == VRTool.PhysicsRaycast);
            }
            if (m_recenterTool != null)
            {
                m_recenterTool.Component = this;
                m_recenterTool.gameObject.SetActive(VrTool == VRTool.Recenter);
            }
            if (m_moveXZTool != null)
            {
                m_moveXZTool.Component = this;
                m_moveXZTool.gameObject.SetActive(VrTool == VRTool.MoveXZ);
            }
            if (m_moveYTool != null)
            {
                m_moveYTool.Component = this;
                m_moveYTool.gameObject.SetActive(VrTool == VRTool.MoveY);
            }

            UpdateGazePointerVisibility();
        }

        private void UpdateGazePointerVisibility()
        {
            if (VrTool == VRTool.None || VrTool == VRTool.Recenter)
            {
                if (m_gazePointer != null)
                {
                    m_gazePointer.gameObject.SetActive(true);
                }

            }
            else
            {
                if (m_gazePointer != null)
                {
                    m_gazePointer.gameObject.SetActive(false);
                }
            }
        }

        protected override void AwakeOverride()
        {
            base.AwakeOverride();
        
            if(GetComponent<RuntimeSelectionInputBase>() == null)
            {
                gameObject.AddComponent<RuntimeSceneVRInput>();
            }

            if(m_recenterTool != null)
            {
                m_recenterTool.Recenter += OnRecenter;
            }
            OnVRToolChanged();

            if (m_cameraRig != Window.Camera.transform.parent)
            {
                m_cameraRig = Window.Camera.transform.parent;
            }

            if (m_cameraRig == null)
            {
                GameObject cameraRig = new GameObject("CameraRig");
                Window.Camera.transform.SetParent(cameraRig.transform);
                m_cameraRig = cameraRig.transform;
            }
        }

        protected override void OnDestroyOverride()
        {
            base.OnDestroyOverride();
        
            if (m_recenterTool != null)
            {
                m_recenterTool.Recenter -= OnRecenter;
            }
        }

        protected override void Start()
        {
            base.Start();
            if(m_gazePointer == null)
            {
                m_gazePointer = FindObjectOfType<RTEVRGazePointer>();
            }
        }

        private void Update()
        {
            m_cameraRig.rotation = Quaternion.Lerp(m_cameraRig.rotation, m_targetRotation, Time.deltaTime * 100);
        }


        private bool m_recenterUsingButton = false;
        private void OnRecenter(Quaternion quaternion)
        {
            m_recenterUsingButton = true;
            m_targetRotation = quaternion;
        }


    }
}
