using UnityEngine;

namespace Battlehub.RTCommon
{
    public class RTEComponent : MonoBehaviour
    {
        protected IRTE m_editor;
        public IRTE Editor
        {
            get { return m_editor; }
        }

        [SerializeField]
        protected RuntimeWindow m_window;

        public virtual RuntimeWindow Window
        {
            get { return m_window; }
            set
            {
                if(m_awaked)
                {
                    throw new System.NotSupportedException("window change is not supported");
                }
                m_editor = IOC.Resolve<IRTE>();
                m_window = value;
            }
        }

        public bool IsWindowActive
        {
            get { return Window == m_editor.ActiveWindow; }
        }

        private bool m_awaked;
        protected bool IsAwaked
        {
            get { return m_awaked; }
        }
        
        private void Awake()
        {
            m_editor = IOC.Resolve<IRTE>();

            if(Window == null)
            {
                Window = GetDefaultWindow();
                if (Window == null)
                {
                    Debug.LogError("m_window == null");
                    enabled = false;
                    return;
                }
            }

            AwakeOverride();
            m_awaked = true;

            if (IsWindowActive)
            {
                OnWindowActivating();
                OnWindowActivated();
            }
            m_editor.ActiveWindowChanging += OnActiveWindowChanging;
            m_editor.ActiveWindowChanged += OnActiveWindowChanged;
        }

        protected virtual RuntimeWindow GetDefaultWindow()
        {
           return m_editor.GetWindow(RuntimeWindowType.Scene);
        }

        protected virtual void AwakeOverride()
        {

        }

        private void OnDestroy()
        {
            if(m_editor != null)
            {
                m_editor.ActiveWindowChanging -= OnActiveWindowChanging;
                m_editor.ActiveWindowChanged -= OnActiveWindowChanged;
            }
            OnDestroyOverride();
            m_editor = null;
        }

        protected virtual void OnDestroyOverride()
        {

        }

        private void OnActiveWindowChanging(RuntimeWindow activatedWindow)
        {
            if(activatedWindow == Window)
            {
                OnWindowActivating();
            }
            else
            {
                OnWindowDeactivating();
            }
        }

        protected virtual void OnActiveWindowChanged(RuntimeWindow deactivatedWindow)
        {
            if (m_editor.ActiveWindow == Window)
            {
                OnWindowActivated();
            }
            else
            {
                OnWindowDeactivated();
            }
        }

        protected virtual void OnWindowActivating()
        {

        }

        protected virtual void OnWindowDeactivating()
        {

        }

        protected virtual void OnWindowActivated()
        {
            
        }

        protected virtual void OnWindowDeactivated()
        {

        }
    }
}

