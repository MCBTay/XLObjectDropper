using Battlehub.RTCommon;
using Battlehub.Utils;
using System.Linq;
using UnityEngine;

namespace Battlehub.RTHandles
{
    public interface IOutlineManager
    {
        IRuntimeSelection Selection
        {
            get;
            set;
        }
        bool ContainsRenderer(Renderer renderer);
        void AddRenderers(Renderer[] renderers);
        void RemoveRenderers(Renderer[] renderers);
        void RecreateCommandBuffer();
    }

    public class OutlineManager : MonoBehaviour, IOutlineManager
    {
        private IRTE m_editor;
        private RuntimeWindow m_sceneWindow;
        private OutlineEffect m_outlineEffect;

        public Camera Camera
        {
            private get;
            set;
        }

        private IRuntimeSelection m_selectionOverride;
        public IRuntimeSelection Selection
        {
            get
            {
                if (m_selectionOverride != null)
                {
                    return m_selectionOverride;
                }

                return m_editor.Selection;
            }
            set
            {
                if (m_selectionOverride != value)
                {
                    if (m_selectionOverride != null)
                    {
                        m_selectionOverride.SelectionChanged -= OnSelectionChanged;
                    }

                    m_selectionOverride = value;
                    if (m_selectionOverride == m_editor.Selection)
                    {
                        m_selectionOverride = null;
                    }

                    if (m_selectionOverride != null)
                    {
                        m_selectionOverride.SelectionChanged += OnSelectionChanged;
                    }
                }
            }
        }

        private void Start()
        {
            m_outlineEffect =  Camera.gameObject.AddComponent<OutlineEffect>();

            m_editor = IOC.Resolve<IRTE>();

            TryToAddRenderers(m_editor.Selection);
            m_editor.Selection.SelectionChanged += OnRuntimeEditorSelectionChanged;

            RTEComponent rteComponent = GetComponentInParent<RTEComponent>();
            if(rteComponent != null)
            {
                m_sceneWindow = rteComponent.Window;
                if(m_sceneWindow != null)
                {
                    m_sceneWindow.IOCContainer.RegisterFallback<IOutlineManager>(this);
                }
            }

            if(RenderPipelineInfo.Type != RPType.Standard)
            {
                Debug.Log("OutlineManager is not supported");
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            if(m_editor != null)
            {
                m_editor.Selection.SelectionChanged -= OnRuntimeEditorSelectionChanged;
            }

            if(m_selectionOverride != null)
            {
                m_selectionOverride.SelectionChanged -= OnSelectionChanged;
            }

            if(m_outlineEffect != null)
            {
                Destroy(m_outlineEffect);
            }

            if (m_sceneWindow != null)
            {
                m_sceneWindow.IOCContainer.UnregisterFallback<IOutlineManager>(this);
            }
        }

        private void OnRuntimeEditorSelectionChanged(Object[] unselectedObject)
        {
            OnSelectionChanged(m_editor.Selection, unselectedObject);
        }

        private void OnSelectionChanged(Object[] unselectedObjects)
        {
            OnSelectionChanged(m_selectionOverride, unselectedObjects);
        }

        private void OnSelectionChanged(IRuntimeSelection selection, Object[] unselectedObjects)
        {
            if (unselectedObjects != null)
            {
                Renderer[] renderers = unselectedObjects.Select(go => go as GameObject).Where(go => go != null).SelectMany(go => go.GetComponentsInChildren<Renderer>(true)).ToArray();
                m_outlineEffect.RemoveRenderers(renderers);
            }
            TryToAddRenderers(selection);
        }

        private void TryToAddRenderers(IRuntimeSelection selection)
        {
            if (selection.gameObjects != null)
            {
                Renderer[] renderers = selection.gameObjects.Where(go => go != null).Select(go => go.GetComponent<ExposeToEditor>()).Where(e => e != null && e.ShowSelectionGizmo && !e.gameObject.IsPrefab() && (e.gameObject.hideFlags & HideFlags.HideInHierarchy) == 0).SelectMany(e => e.GetComponentsInChildren<Renderer>().Where(r => (r.gameObject.hideFlags & HideFlags.HideInHierarchy) == 0)).ToArray();
                m_outlineEffect.AddRenderers(renderers);
            }
        }

        public bool ContainsRenderer(Renderer renderer)
        {
            return m_outlineEffect.ContainsRenderer(renderer);
        }

        public void AddRenderers(Renderer[] renderers)
        {
            m_outlineEffect.AddRenderers(renderers);
        }

        public void RemoveRenderers(Renderer[] renderers)
        {
            m_outlineEffect.RemoveRenderers(renderers);
        }

        public void RecreateCommandBuffer()
        {
            m_outlineEffect.RecreateCommandBuffer();
        }
    }
}

