using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Battlehub.RTCommon
{
    public class EmuTouchEventArgs : EventArgs
    {
        public PointerEventData PointerData
        {
            get;
            set;
        }
    }

    public class EmuTouchControl : MonoBehaviour
    {
        private Text m_txtNum;
        private Image m_graphics;
        private Color m_selectedColor = new Color(1, 0.5f, 0);
        private Color m_deselectedColor = Color.white;

        public bool IsDown;
        public bool IsUp;
        public bool IsPressed;

        private int m_index;
        public int Index
        {
            get { return m_index; }
            set
            {
                m_index = value;
                if(m_txtNum != null)
                {
                    m_txtNum.text = value.ToString();
                }
            }
        }

        protected void Awake()
        {
            m_graphics = GetComponent<Image>();
            m_txtNum = GetComponentInChildren<Text>();
        }

        public void Pressed()
        {
            m_graphics.color = m_selectedColor;
        }

        public void Released()
        {
            m_graphics.color = m_deselectedColor;
        }

    }
}


