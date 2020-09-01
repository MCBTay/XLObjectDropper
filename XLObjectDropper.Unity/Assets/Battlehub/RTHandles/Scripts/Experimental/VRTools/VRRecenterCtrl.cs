using System;
using UnityEngine;
using UnityEngine.UI;

namespace Battlehub.RTHandles
{
    public class VRRecenterCtrl : MonoBehaviour
    {
        [SerializeField]
        private Button m_btnRecenter = null;

        [SerializeField]
        private Button m_btnUp = null;

        [SerializeField]
        private Button m_btnDown = null;

        [SerializeField]
        private Button m_buttonLeft = null;

        [SerializeField]
        private Button m_buttonRight = null;

        public event EventHandler Recenter;
        public event EventHandler Up;
        public event EventHandler Down;
        public event EventHandler Left;
        public event EventHandler Right;


        [SerializeField]
        private Text m_text;

        public string Text
        {
            get
            {
                if(m_text == null)
                {
                    return null;
                }
                return m_text.text;
            }
            set
            {
                if(m_text == null)
                {
                    return;
                }
                m_text.text = value;

            }
        }

        private void Awake()
        {
            if(m_text == null)
            {
                if(m_btnRecenter != null)
                {
                    m_text = m_btnRecenter.GetComponentInChildren<Text>(true);
                }
            }

            if (m_btnRecenter != null)
            {
                m_btnRecenter.onClick.AddListener(OnRecenter);
            }

            if(m_btnUp != null)
            {
                m_btnUp.onClick.AddListener(OnUp);
            }

            if(m_btnDown != null)
            {
                m_btnDown.onClick.AddListener(OnDown);
            }

            if(m_buttonLeft != null)
            {
                m_buttonLeft.onClick.AddListener(OnLeft);
            }

            if(m_buttonRight != null)
            {
                m_buttonRight.onClick.AddListener(OnRight);
            }
        }

        private void OnDestroy()
        {
            if (m_btnRecenter != null)
            {
                m_btnRecenter.onClick.RemoveListener(OnRecenter);
            }

            if (m_btnUp != null)
            {
                m_btnUp.onClick.RemoveListener(OnUp);
            }

            if (m_btnDown != null)
            {
                m_btnDown.onClick.RemoveListener(OnDown);
            }

            if (m_buttonLeft != null)
            {
                m_buttonLeft.onClick.RemoveListener(OnLeft);
            }

            if (m_buttonRight != null)
            {
                m_buttonRight.onClick.RemoveListener(OnRight);
            }
        }

        private void OnRecenter()
        {
            if(Recenter != null)
            {
                Recenter(this, EventArgs.Empty);
            }
        }

        private void OnUp()
        {
            if(Up != null)
            {
                Up(this, EventArgs.Empty);
            }
        }

        private void OnDown()
        {
            if(Down != null)
            {
                Down(this, EventArgs.Empty);
            }
        }

        private void OnLeft()
        {
            if(Left != null)
            {
                Left(this, EventArgs.Empty);
            }
        }

        private void OnRight()
        {
            if(Right != null)
            {
                Right(this, EventArgs.Empty);
            }
        }

    }

}
