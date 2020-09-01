using UnityEngine;

namespace Battlehub.RTCommon
{
    public class SpriteGizmo : MonoBehaviour, IGL
    {
        public Material Material;
        [SerializeField, HideInInspector]
        private SphereCollider m_collider;
        private SphereCollider m_destroyedCollider;
        [SerializeField]
        private float m_scale = 1.0f;
        public float Scale
        {
            get { return m_scale; }
            set
            {
                if(m_scale != value)
                {
                    m_scale = value;
                    UpdateCollider();
                }
            }
        }

        private void Awake()
        {
            if (GLRenderer.Instance == null)
            {
                GameObject glRenderer = new GameObject();
                glRenderer.name = "GLRenderer";
                glRenderer.AddComponent<GLRenderer>();
            }
        }     

        private void OnEnable()
        {
            GLRenderer glRenderer = GLRenderer.Instance;
            if (glRenderer)
            {
                glRenderer.Add(this);
            }

            m_collider = GetComponent<SphereCollider>();

            if (m_collider == null || m_collider == m_destroyedCollider)
            {
                m_collider = gameObject.AddComponent<SphereCollider>();
            }
            if (m_collider != null)
            {
                if (m_collider.hideFlags == HideFlags.None)
                {
                    m_collider.hideFlags = HideFlags.HideInInspector;
                }

                UpdateCollider();
            }
        }

        private void OnDisable()
        {
            GLRenderer glRenderer = GLRenderer.Instance;
            if (glRenderer)
            {
                glRenderer.Remove(this);
            }

            if(m_collider != null)
            {
                Destroy(m_collider);
                m_destroyedCollider = m_collider;
                m_collider = null;
            }
        }

        private void UpdateCollider()
        {
            if(m_collider != null)
            {
                m_collider.radius = 0.25f * m_scale;
            }
        }


        void IGL.Draw(int cullingMask, Camera camera)
        {
            Material.SetPass(0);
            RuntimeGraphics.DrawQuad(Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one * m_scale));
        }
    }
}

