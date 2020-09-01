using Battlehub.RTCommon;
using UnityEngine;

namespace Battlehub.RTHandles
{
    [DefaultExecutionOrder(5)]
    public class BaseHandleModel : RTEComponent
    {
        private RuntimeHandlesComponent m_appearance;
        public RuntimeHandlesComponent Appearance
        {
            get { return m_appearance; }
            set { m_appearance = value; }
        }

        public RTHColors Colors
        {
            get { return m_appearance.Colors; }
        }

        private float m_modelScale = 1.0f;
        public float ModelScale
        {
            get { return m_modelScale;  }
            set
            {
                if(m_modelScale != value)
                {
                    m_modelScale = value;
                    if(enabled && gameObject.activeSelf)
                    {
                        UpdateModel();
                    }
                }   
            }
        }

        private float m_selectionMargin = 1.0f;
        public float SelectionMargin
        {
            get { return m_selectionMargin; }
            set
            {
                if(m_selectionMargin != value)
                {
                    m_selectionMargin = value;
                    if(enabled && gameObject.activeSelf)
                    {
                        UpdateModel();
                    }
                }
            }
        }

        protected RuntimeHandleAxis m_selectedAxis = RuntimeHandleAxis.None;
        protected LockObject m_lockObj = new LockObject();
        
        protected override void AwakeOverride()
        {
            base.AwakeOverride();
            RuntimeGraphicsLayer graphicsLayer = Window.GetComponent<RuntimeGraphicsLayer>();
            if (graphicsLayer == null)
            {
                graphicsLayer = Window.gameObject.AddComponent<RuntimeGraphicsLayer>();
            }

            SetLayer(transform, graphicsLayer.Window.Editor.CameraLayerSettings.RuntimeGraphicsLayer + Window.Index);
        }

        private void SetLayer(Transform t, int layer)
        {
            t.gameObject.layer = layer;
            foreach (Transform child in t)
            {
                SetLayer(child, layer);
            }
        }

        public virtual void SetLock(LockObject lockObj)
        { 
            if(lockObj == null)
            {
                lockObj = new LockObject();
            }
            m_lockObj = lockObj;
        }

        public virtual void Select(RuntimeHandleAxis axis)
        {
            m_selectedAxis = axis;   
        }

        public virtual void SetScale(Vector3 scale)
        {

        }

        public virtual RuntimeHandleAxis HitTest(Ray ray, out float distance)
        {
            distance = float.PositiveInfinity;
            return RuntimeHandleAxis.None;
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnEnable()
        {
            UpdateModel();
        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void Update()
        {

        }

        public virtual void UpdateModel()
        {

        }

     
    }
}
