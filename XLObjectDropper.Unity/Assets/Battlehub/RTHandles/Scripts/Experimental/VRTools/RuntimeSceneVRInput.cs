using Battlehub.RTCommon;
using UnityEngine;

namespace Battlehub.RTHandles
{
    public class RuntimeSceneVRInput : RuntimeSelectionInput
    {
        private float m_countdownStartT = float.MaxValue;
        
        [SerializeField]
        private RuntimeSceneVRMenu m_menu = null;

        public RuntimeSceneVRComponent Component
        {
            get { return (RuntimeSceneVRComponent)m_component; }
        }

        protected override void Awake()
        {
            base.Awake();
            if(m_menu != null)
            {
                m_menu.Opened += OnOpened;
                m_menu.MoveXY += OnMoveXY;
                m_menu.MoveZ += OnMoveZ;
                m_menu.Recenter += OnRecenter;
                m_menu.CurvedRaycast += OnCurvedRaycast;
                m_menu.PositionHandle += OnPositionHandle;
                m_menu.RotationHandle += OnRotationHandle;
                m_menu.ScaleHandle += OnScaleHandle;
                m_menu.Closed += OnClosed;
            }
        }        

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if(m_menu != null)
            {
                m_menu.Opened -= OnOpened;
                m_menu.MoveXY -= OnMoveXY;
                m_menu.MoveZ -= OnMoveZ;
                m_menu.Recenter -= OnRecenter;
                m_menu.CurvedRaycast -= OnCurvedRaycast;
                m_menu.PositionHandle -= OnPositionHandle;
                m_menu.RotationHandle -= OnRotationHandle;
                m_menu.ScaleHandle -= OnScaleHandle;
                m_menu.Closed -= OnClosed;
            }
        }

        private void OnOpened(object sender, System.EventArgs e)
        {
            Component.Lock();
        }

        private void OnRecenter(object sender, System.EventArgs e)
        {
            Component.BeginRecenter();
        }

        private void OnMoveXY(object sender, System.EventArgs e)
        {
            Component.BeginMoveXY();
        }

        private void OnMoveZ(object sender, System.EventArgs e)
        {
            Component.BeginMoveZ();
        }

        private void OnCurvedRaycast(object sender, System.EventArgs e)
        {
            Component.BeginCurvedRaycast();
        }

        private void OnPositionHandle(object sender, System.EventArgs e)
        {
            Component.SwitchToPositionHandle();
        }

        private void OnRotationHandle(object sender, System.EventArgs e)
        {
            Component.SwitchToRotationHandle();
        }

        private void OnScaleHandle(object sender, System.EventArgs e)
        {
            Component.SwitchToScaleHandle();
        }

        private void OnClosed(object sender, System.EventArgs e)
        {
            Component.Unlock();
        }


        protected override void LateUpdate()
        {
            base.LateUpdate();

            if (Time.time > m_countdownStartT)
            {
                if (Component.Progress == 0)
                {
                    Component.BeginShowProgress();
                }
                Component.Progress = (Time.time - m_countdownStartT) * 5f;
                if (Component.Progress >= 1)
                {
                    m_countdownStartT = float.PositiveInfinity;
                    m_menu.IsOpened = true;
                }
            }

            IInput input = m_component.Editor.Input;
            if(input.GetPointerDown(0))
            {
                if (m_component.Editor.Tools.ActiveTool == null)
                {
                    BeginOpenMenu();
                }
            }
            else if (input.GetPointerUp(0))
            {
                if(m_component.Editor.Tools.ActiveTool == null)
                {
                    if (!m_menu.IsOpened)
                    {
                        Component.Action();
                    }
                    CloseMenu();
                    
                }
            }
        }

        private void BeginOpenMenu()
        {
            m_countdownStartT = Time.time + 0.2f;
            Component.Progress = 0;
        }

        private void CloseMenu()
        {
            Component.EndShowProgress();

            if (Component.Progress < 1)
            {
                Component.Progress = 0;
            }

            m_countdownStartT = float.PositiveInfinity;
            if(m_menu != null)
            {
                m_menu.IsOpened = false;
            }
        }

    }
}

