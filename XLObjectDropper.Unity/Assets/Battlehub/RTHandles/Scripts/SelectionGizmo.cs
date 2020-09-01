using UnityEngine;

using Battlehub.RTCommon;

namespace Battlehub.RTHandles
{
    /// <summary>
    /// Draws bounding box of selected object
    /// </summary>
    public class SelectionGizmo : MonoBehaviour, IGL
    {
        [HideInInspector]
        public bool Internal_Destroyed = false;
        public RuntimeHandlesComponent Appearance;
        private ExposeToEditor m_exposeToEditor;

        private RuntimeWindow m_window;
        public virtual RuntimeWindow Window
        {
            get { return m_window; }
            set { m_window = value; }
        }

        [SerializeField]
        private GameObject m_selectionGizmoModel;
        public GameObject SelectionGizmoModel
        {
            get { return m_selectionGizmoModel; }
            set
            {
                m_selectionGizmoModel = value;
                if(m_selectionGizmoModel != null)
                {
                    Bounds bounds = m_exposeToEditor.Bounds;
                    m_selectionGizmoModel.transform.localScale = bounds.size;
                    m_selectionGizmoModel.transform.localPosition = bounds.center;

                    if (GLRenderer.Instance != null)
                    {
                        GLRenderer.Instance.Remove(this);
                    }
                }
            }
        }

        private IRTE m_editor;

        private void Awake()
        {
            m_editor = IOC.Resolve<IRTE>();
            m_editor.IsOpenedChanged += OnIsOpenedChanged;
            m_exposeToEditor = GetComponent<ExposeToEditor>();
            RuntimeHandlesComponent.InitializeIfRequired(ref Appearance);
        }

        private void OnDestroy()
        {
            if(m_editor != null)
            {
                m_editor.IsOpenedChanged -= OnIsOpenedChanged;
            }

            if(m_selectionGizmoModel != null)
            {
                Destroy(m_selectionGizmoModel);
            }
        }

        private void Start()
        {
            if (m_window == null)
            {
                m_window = m_editor.GetWindow(RuntimeWindowType.Scene);
                if(m_window == null)
                {
                    Debug.LogError("m_window == null");
                    enabled = false;
                    return;
                }
            }

            if (m_selectionGizmoModel == null)
            {
                if (GLRenderer.Instance == null)
                {
                    GameObject glRenderer = new GameObject();
                    glRenderer.name = "GLRenderer";
                    glRenderer.AddComponent<GLRenderer>();
                }

                if (m_exposeToEditor != null && m_exposeToEditor.ShowSelectionGizmo)
                {
                    GLRenderer.Instance.Add(this);
                }
            }

            if (!m_editor.Selection.IsSelected(gameObject))
            {
                Destroy(this);
            }
        }

        private void OnEnable()
        {
            if (m_selectionGizmoModel == null)
            {
                if (m_exposeToEditor != null && m_exposeToEditor.ShowSelectionGizmo)
                {
                    if (GLRenderer.Instance != null)
                    {
                        GLRenderer.Instance.Add(this);
                    }
                }
            }
        }

        private void OnDisable()
        {
            if (GLRenderer.Instance != null)
            {
                GLRenderer.Instance.Remove(this);
            }
        }

        public void Draw(int cullingMask, Camera camera)
        {
            if (m_editor.Tools.ShowSelectionGizmos)
            {
                if ((cullingMask & (1 << (m_editor.CameraLayerSettings.RuntimeGraphicsLayer + Window.Index))) == 0)
                {
                    return;
                }

                Bounds bounds = m_exposeToEditor.Bounds;
                Transform trform = m_exposeToEditor.BoundsObject.transform;
                Appearance.DrawBounds(camera, ref bounds, trform.position, trform.rotation, trform.lossyScale);
            }
        }


        private void OnIsOpenedChanged()
        {
            if (m_editor != null && !m_editor.IsOpened)
            {
                Destroy(this);
            }
        }

      

    }
}
