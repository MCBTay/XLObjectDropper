using System;
using UnityEngine;
using UnityEngine.Events;

namespace Battlehub.RTHandles
{
    public class RuntimeSceneVRMenu : MonoBehaviour
    {
        [SerializeField]
        private PointerOverButton m_recenterButton = null;

        [SerializeField]
        private PointerOverButton m_moveXZButton = null;

        [SerializeField]
        private PointerOverButton m_moveYButton = null;

        [SerializeField]
        private PointerOverButton m_positionHandleButton = null;

        [SerializeField]
        private PointerOverButton m_rotationHandleButton = null;

        [SerializeField]
        private PointerOverButton m_scaleHandleButton = null;

        public bool IsOpened
        {
            get { return gameObject.activeSelf; }
            set
            {
                if(IsOpened != value)
                {
                    gameObject.SetActive(value);
                    if(value)
                    {
                        if(Opened != null)
                        {
                            Opened(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        if (Closed != null)
                        {
                            Closed(this, EventArgs.Empty);
                        }
                    }
                }
            }
        }

        public event EventHandler Opened;
        public event EventHandler Recenter;
        public event EventHandler CurvedRaycast;
        public event EventHandler MoveXY;
        public event EventHandler MoveZ;
        public event EventHandler PositionHandle;
        public event EventHandler RotationHandle;
        public event EventHandler ScaleHandle;
        public event EventHandler Closed;

        private void Start()
        {
            if(m_recenterButton != null)
            {
                m_recenterButton.onClick.AddListener(OnRecenterClick);
            }
            
            if(m_moveXZButton != null)
            {
                m_moveXZButton.onClick.AddListener(OnMoveXZClick);
            }
            if(m_moveYButton)
            {
                m_moveYButton.onClick.AddListener(OnMoveYClick);
            }
            if(m_positionHandleButton != null)
            {
                m_positionHandleButton.onClick.AddListener(OnPositionHandleClick);
            }
            if(m_rotationHandleButton != null)
            {
                m_rotationHandleButton.onClick.AddListener(OnRotationHandleClick);
            }
            if(m_scaleHandleButton != null)
            {
                m_scaleHandleButton.onClick.AddListener(OnScaleHandleClick);
            }
        }

        private void OnDestroy()
        {
            RemoveListener(m_recenterButton, OnRecenterClick);
            RemoveListener(m_moveXZButton, OnMoveXZClick);
            RemoveListener(m_moveYButton, OnMoveYClick);
            RemoveListener(m_positionHandleButton, OnPositionHandleClick);
            RemoveListener(m_rotationHandleButton, OnRotationHandleClick);
            RemoveListener(m_scaleHandleButton, OnScaleHandleClick);
        }

        private void RemoveListener(PointerOverButton button, UnityAction listener)
        {
            if(button != null)
            {
                button.onClick.RemoveListener(listener);
            }
        }

        private void OnRecenterClick()
        {
            if (Recenter != null)
            {
                Recenter(this, EventArgs.Empty);
            }
            IsOpened = false;
        }

        private void OnCurvedRaycastClick()
        {
            if (CurvedRaycast != null)
            {
                CurvedRaycast(this, EventArgs.Empty);
            }
            IsOpened = false;
        }

        private void OnMoveXZClick()
        {
            if (MoveXY != null)
            {
                MoveXY(this, EventArgs.Empty);
            }

            IsOpened = false;
        }

        private void OnMoveYClick()
        {
            if (MoveZ != null)
            {
                MoveZ(this, EventArgs.Empty);
            }

            IsOpened = false;
        }

        private void OnPositionHandleClick()
        {
            if (PositionHandle != null)
            {
                PositionHandle(this, EventArgs.Empty);
            }

            IsOpened = false;
        }

        private void OnRotationHandleClick()
        {
            if (RotationHandle != null)
            {
                RotationHandle(this, EventArgs.Empty);
            }

            IsOpened = false;
        }

        private void OnScaleHandleClick()
        {
            if (ScaleHandle != null)
            {
                ScaleHandle(this, EventArgs.Empty);
            }

            IsOpened = false;
        }

        private void OnCloseClick()
        {
            IsOpened = false;
        }
    }
}


