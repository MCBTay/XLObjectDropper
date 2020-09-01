using Battlehub.RTCommon;
using System.Linq;
using UnityEngine;

namespace Battlehub.RTHandles
{
    [DefaultExecutionOrder(-60)]
    public class RuntimeSelectionInputBase : MonoBehaviour
    {
        protected RuntimeSelectionComponent m_component;
        
        protected virtual void Awake()
        {

        }

        protected virtual void Start()
        {
            m_component = GetComponent<RuntimeSelectionComponent>();
        }

        protected virtual void OnDestroy()
        {

        }

        protected virtual void LateUpdate()
        {
            if (!m_component.IsWindowActive || !m_component.Window.IsPointerOver)
            {
                return;
            }

            if (SelectAction())
            {
                SelectGO();
            }
        }

        protected virtual bool SelectAction()
        {
            return m_component.Editor.Input.GetPointerDown(0);
        }

        protected virtual void SelectGO()
        {
            RuntimeTools tools = m_component.Editor.Tools;
            IRuntimeSelection selection = m_component.Selection;
            IInput input = m_component.Editor.Input;

            if (tools.ActiveTool != null && tools.ActiveTool != m_component.BoxSelection)
            {
                return;
            }

            if (tools.IsViewing)
            {
                return;
            }

            if (!selection.Enabled)
            {
                return;
            }

            OnSelectGO();
        }

        protected virtual void OnSelectGO()
        {
            m_component.SelectGO(false, false);
        }

        protected virtual void SelectAll()
        {
            m_component.SelectAll();
        }
    }

}
