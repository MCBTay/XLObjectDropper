using Battlehub.RTCommon;
using System.Collections.Generic;
using UnityEngine;

namespace Battlehub.RTHandles
{
    [RequireComponent(typeof(LineRenderer))]
    public class VRMoveXZTool : VRBaseTool
    {
        public enum InterpolationType
        {
            None,
            Custom,
            Linear,
            Exponential,
            Sine,
        }


        [SerializeField]
        private AnimationCurve m_interpolationCurve = null;
        [SerializeField]
        public InterpolationType m_interpolationType = InterpolationType.Exponential;
        [SerializeField]
        private float m_maxDistance = 100;
        [SerializeField]
        private Vector3 m_gravityDirection = Vector3.down * 2f;
        [SerializeField]
        private int m_smoothness = 10;
        [SerializeField]
        private Vector3 m_originOffset = Vector3.up * 0.2f;


        private Plane m_dragPlane;
        private List<Vector3> points = new List<Vector3>();
        private List<Vector3> prevPoints = new List<Vector3>();
        private Vector3 m_prevOrigin;
        private Vector3 m_prevForward;
        private LineRenderer m_lineRenderer;

        protected override void AwakeOverride()
        {
            base.AwakeOverride();
            m_lineRenderer = GetComponent<LineRenderer>();  
        }
        
        protected virtual void Start()
        {
            InterploatedCurveCast(m_prevOrigin, m_prevForward);
        }

        private void OnEnable()
        {
            m_dragPlane = new Plane(Vector3.up, Vector3.zero);
            Transform camera = Window.Camera.transform;
            m_prevOrigin = camera.TransformPoint(m_originOffset);
            m_prevForward = camera.forward;
        }

        private void Update()
        {
            Transform camera = Window.Camera.transform;
            Vector3 origin = camera.TransformPoint(m_originOffset);
            Vector3 forward = camera.forward;

            if(m_prevOrigin != origin && m_prevForward != forward)
            {
                InterploatedCurveCast(origin, forward);
                m_prevOrigin = origin;
                m_prevForward = forward;
            }   
        }

        private void InterploatedCurveCast(Vector3 origin, Vector3 dir)
        {
            RaycastHit hit;
            CurveCast(origin, dir, m_gravityDirection, m_smoothness, out hit, m_maxDistance);

            Interpolate();

            transform.position = points[points.Count - 1];

            if (m_lineRenderer != null)
            {
                m_lineRenderer.positionCount = points.Count;
                for (int i = 0; i < points.Count; ++i)
                {
                    m_lineRenderer.SetPosition(i, points[i]);
                }
            }
        }

        private void Interpolate()
        {
            if (prevPoints.Count == 0)
            {
                for(int i = 0; i < points.Count; ++i)
                {
                    Vector3 v = points[i];
                    prevPoints.Add(new Vector3(v.x, v.y + Mathf.InverseLerp(0, points.Count, points.IndexOf(v)), v.z));
                }
            }
            float lengthI = points.Count;
            for (int i = 0; i < lengthI; i++)
            {
                float t = default(float);
                float interpolVal = i / lengthI;
                switch (m_interpolationType)
                {
                    case InterpolationType.Custom:
                        t = m_interpolationCurve.Evaluate(interpolVal);
                        break;
                    case InterpolationType.Linear:
                        t = interpolVal;
                        break;
                    case InterpolationType.Exponential:
                        t = Mathf.Pow(interpolVal, 2);
                        break;
                    case InterpolationType.Sine:
                        t = Mathf.Sin(interpolVal * Mathf.PI) * .9f;
                        break;
                    case InterpolationType.None:
                        t = 0;
                        break;
                }
                points[i] = Vector3.Lerp(points[i], prevPoints.Count >= 1 ? prevPoints[i > prevPoints.Count - 1 ? prevPoints.Count - 1 : i] : points[i], t * .9f);
            }
            prevPoints.Clear();
            for(int i = 0; i < points.Count; ++i)
            {
                prevPoints.Add(points[i]);
            } 
        }

        private bool CurveCast(Vector3 origin, Vector3 direction, Vector3 gravityDirection, int smoothness, out RaycastHit hitInfo, float maxDistance)
        {
            if (maxDistance == Mathf.Infinity) maxDistance = 500;
            Vector3 currPos = origin, hypoPos = origin, hypoVel = direction.normalized / smoothness;
            points.Clear();
            RaycastHit hit;
            float curveCastLength = 0;

            do
            {
                points.Add(hypoPos);
                currPos = hypoPos;
                hypoPos = currPos + hypoVel + (gravityDirection * Time.fixedDeltaTime / (smoothness * smoothness));
                hypoVel = hypoPos - currPos;
                curveCastLength += hypoVel.magnitude;
            }
            while (Raycast(currPos, hypoVel, out hit, hypoVel.magnitude) == false && curveCastLength < maxDistance);
            hitInfo = hit;
            return Raycast(currPos, hypoVel, out hit, hypoVel.magnitude);
        }

        protected virtual bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance)
        {
            hit = new RaycastHit();

            float distance;
            Ray ray = new Ray(origin, direction);
            if (!m_dragPlane.Raycast(ray, out distance))
            {
                return false;
            }

            if (distance > maxDistance)
            {
                return false;
            }

            hit.point = ray.GetPoint(distance);
            hit.normal = m_dragPlane.normal;

            return true;
        }
    }
}


