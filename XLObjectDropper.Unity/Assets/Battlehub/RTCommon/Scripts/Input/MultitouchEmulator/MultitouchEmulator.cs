using System;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.RTCommon
{
    [DefaultExecutionOrder(-100)]
    public class MultitouchEmulator : RTEComponent
    {
        [SerializeField]
        private EmuTouchControl m_touchPrefab = null;

        private EmuTouchControl[] m_touches = new EmuTouchControl[5];
        private EmuTouchControl m_selectedTouch;


        protected override void AwakeOverride()
        {
            base.AwakeOverride();
            Editor.Input.MultitouchEmulator = this;
        }

        private void Update()
        {
            if(Editor.ActiveWindow == null)
            {
                return;
            }

            for (int i = 0; i < m_touches.Length; ++i)
            {
                if (m_touches[i] != null && m_touches[i].IsUp)
                {
                    Destroy(m_touches[i].gameObject);
                    m_touches[i] = null;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (m_selectedTouch != null)
                {
                    m_selectedTouch.Released();
                }

                m_selectedTouch = null;
                return;
            }

            if (!Editor.ActiveWindow.IsPointerOver)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                EmuTouchControl touch = GetSelectedTouchControl();
                if (touch == null)
                {
                    int touchIndex = -1;
                    for (int i = 0; i < m_touches.Length; ++i)
                    {
                        if (m_touches[i] == null)
                        {
                            touchIndex = i;
                            break;
                        }
                    }

                    if (touchIndex >= 0)
                    {
                        touch = Instantiate(m_touchPrefab, Editor.Input.GetPointerXY(0), Quaternion.identity);
                        touch.transform.SetParent(transform, true);
                        touch.Index = touchIndex;
                        m_touches[touchIndex] = touch;
                    }

                    if (touch != null)
                    {
                        touch.IsDown = true;
                        touch.IsPressed = true;
                    }
                }
                
               
                m_selectedTouch = touch;
                if(m_selectedTouch != null)
                {
                    ShowTooltip();
                    m_selectedTouch.Pressed();
                }
            }
            else
            {
                if(m_selectedTouch != null)
                {
                    m_selectedTouch.IsDown = false;
                }
            }
            
            if (m_selectedTouch != null)
            {
                m_selectedTouch.transform.position = Input.mousePosition;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HideTooltip();

                for(int i = 0; i < m_touches.Length; ++i)
                {
                    if(m_touches[i] != null)
                    {
                        m_touches[i].IsDown = false;
                        m_touches[i].IsPressed = false;
                        m_touches[i].IsUp = true;
                    }
                }
            }
        }

        public EmuTouchControl GetSelectedTouchControl()
        {
            Vector3 mousePosition = Input.mousePosition;
            for(int i = 0; i < m_touches.Length; ++i)
            {
                EmuTouchControl touch = m_touches[i];
                if(touch != null)
                {
                    RectTransform rt = touch.GetComponent<RectTransform>();
                    if(rt != null)
                    {
                        float radius = Mathf.Min(rt.rect.width, rt.rect.height) / 2;
                        float distance = (mousePosition - rt.position).magnitude;
                        if(distance < radius)
                        {
                            return touch;
                        }
                    }
                }
            }
            return null;
        }

        public Vector3 GetPosition(int index)
        {
            if (index < 0 || index >= m_touches.Length)
            {
                return Vector3.zero;
            }

            if(m_touches[index] != null)
            {
                return m_touches[index].transform.position;
            }

            return Vector3.zero;
        }

        public bool IsTouchDown(int index)
        {
            if(index < 0 || index >= m_touches.Length)
            {
                return false;
            }

            return m_touches[index] != null && m_touches[index].IsDown;
        }

        public bool IsTouchUp(int index)
        {
            if (index < 0 || index >= m_touches.Length)
            {
                return false;
            }

            return m_touches[index] != null && m_touches[index].IsUp;
        }

        public bool IsTouch(int index)
        {
            if (index < 0 || index >= m_touches.Length)
            {
                return false;
            }

            return m_touches[index] != null && m_touches[index].IsPressed;
        }


        private GameObject m_tooltip;
        private void ShowTooltip()
        {
            if(m_tooltip != null)
            {
                return;
            }

            m_tooltip = new GameObject("Tooltip");
            m_tooltip.transform.SetParent(transform, false);

            Text txt = m_tooltip.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt.text = "Hit 'Esc' to release";
            txt.color = Color.black;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.rectTransform.sizeDelta = new Vector2(200, 50);
        }

        private void HideTooltip()
        {
            if(m_tooltip == null)
            {
                return;
            }
            Destroy(m_tooltip);
        }
    }
}


