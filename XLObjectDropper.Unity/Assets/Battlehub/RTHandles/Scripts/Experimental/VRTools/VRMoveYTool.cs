using UnityEngine;

namespace Battlehub.RTHandles
{
    [RequireComponent(typeof(LineRenderer))]
    public class VRMoveYTool : VRBaseTool
    {
        private Plane m_dragPlane;

        [SerializeField]
        private float m_distance = 10;

        private Vector3 m_prevOrigin;
        private Vector3 m_prevForward;

        private LineRenderer m_lineRenderer;

        protected override void AwakeOverride()
        {
            base.AwakeOverride();
            gameObject.SetActive(false);
            m_lineRenderer = GetComponent<LineRenderer>();

            Transform camera = Window.Camera.transform;
            m_prevOrigin = camera.position;
            m_prevForward = camera.forward;
            m_lineRenderer.positionCount = 2;
            UpdateTool();
        }

        private void Update()
        {
            if(!IsWindowActive)
            {
                return;
            }

            Transform camera = Window.Camera.transform;
            if (m_prevForward != camera.forward || m_prevOrigin != camera.position)
            {
                UpdateTool();
                m_prevOrigin = camera.position;
                m_prevForward = camera.forward;
            }
            
        }

        private void UpdateTool()
        {
            Ray ray = Window.Pointer.Ray;

            Vector3 direction = ray.direction;
            direction.y = 0;
            direction.Normalize();

            Vector3 origin = ray.origin + direction * m_distance;
            m_dragPlane = new Plane(-direction, origin);

            float distance;
            if (m_dragPlane.Raycast(ray, out distance))
            {
                transform.position = ray.GetPoint(distance);
                m_lineRenderer.SetPosition(0, origin);
                m_lineRenderer.SetPosition(1, transform.position);
            }
        }
    }
}

