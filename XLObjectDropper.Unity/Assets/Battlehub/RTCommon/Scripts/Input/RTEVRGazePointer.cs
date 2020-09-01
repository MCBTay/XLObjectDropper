using UnityEngine;

namespace Battlehub.RTCommon
{
    public class RTEVRGazePointer : MonoBehaviour
    {
        private OVRGazePointer m_ovrGazePointer;
        
        private IRTE m_editor;
        private Camera m_camera;

        private SpriteMask m_spriteMask;
        private SpriteRenderer m_spriteRenderer;
        
        public float Progress
        {
            get { return m_spriteMask.alphaCutoff; }
            set { m_spriteMask.alphaCutoff = value; }
        }

        private void Start()
        {
            m_camera = GetComponentInParent<Camera>();
            m_editor = IOC.Resolve<IRTE>();
          
            m_ovrGazePointer = GetComponentInChildren<OVRGazePointer>();
            if(m_ovrGazePointer == null)
            {
                GameObject ovrGazePointer = new GameObject("OVRGazePointer");
                ovrGazePointer.SetActive(false);
                ovrGazePointer.transform.SetParent(transform, false);

                m_ovrGazePointer = ovrGazePointer.AddComponent<OVRGazePointer>();
            }

            m_spriteRenderer = m_ovrGazePointer.GetComponentInChildren<SpriteRenderer>();
            if(m_spriteRenderer == null)
            {
                GameObject gazeIcon = new GameObject("GazeIcon");
                gazeIcon.layer = m_editor.CameraLayerSettings.RuntimeGraphicsLayer;
                gazeIcon.transform.SetParent(m_ovrGazePointer.transform, false);

                SpriteRenderer renderer = gazeIcon.AddComponent<SpriteRenderer>();
                renderer.sortingOrder = short.MaxValue;
                renderer.sharedMaterial = Resources.Load<Material>("RTC_GazeIcon");
                renderer.sprite = Resources.Load<Sprite>("RTC_GazeIcon");
                renderer.color = Color.black;

                SpriteMask spriteMask = gazeIcon.AddComponent<SpriteMask>();
                spriteMask.sprite = Resources.Load<Sprite>("RTC_GazeMask");

                m_spriteRenderer = renderer;
                m_spriteMask = spriteMask;
            }
            else
            {
                if(!m_spriteRenderer.GetComponent<SpriteMask>())
                {
                    m_spriteMask = m_spriteRenderer.gameObject.AddComponent<SpriteMask>();
                    m_spriteMask.sprite = Resources.Load<Sprite>("RTC_GazeMask");
                }
            }

            m_spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            m_spriteMask.alphaCutoff = 1;

            m_ovrGazePointer.gameObject.SetActive(true);

            m_ovrGazePointer.hideByDefault = false;
            m_ovrGazePointer.showTimeoutPeriod = 0;
            m_ovrGazePointer.hideTimeoutPeriod = 0;
            m_ovrGazePointer.dimOnHideRequest = true;
            m_ovrGazePointer.depthScaleMultiplier = 0.07f;
            m_ovrGazePointer.cursorRadius = 1;

            m_ovrGazePointer.rayTransform = m_camera.transform;
            m_ovrGazePointer.SetPosition(m_camera.transform.position + m_camera.transform.forward * 10);

        }

        private void Update()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;

            Transform t = m_ovrGazePointer.rayTransform;            
            if(t == null)
            {
                return;
            }

            Ray ray = new Ray(t.position + t.forward * m_camera.nearClipPlane, t.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                m_ovrGazePointer.SetPosition(hit.point);
            }
        }
    }
}


