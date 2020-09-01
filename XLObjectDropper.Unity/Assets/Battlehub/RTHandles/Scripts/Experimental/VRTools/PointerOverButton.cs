using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
namespace Battlehub.RTHandles
{
    public class PointerOverButton : Selectable
    {
        [SerializeField]
        public UnityEvent onClick;

        private bool m_isPointerOver = false;

        public override void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("m_isPointerOver = false");
            m_isPointerOver = false;
            base.OnPointerExit(eventData);

        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            m_isPointerOver = true;
            base.OnPointerEnter(eventData);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (m_isPointerOver)
            {
                onClick.Invoke();
            }
            m_isPointerOver = false;
        }
    }

}
