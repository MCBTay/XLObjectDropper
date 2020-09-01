using UnityEngine;

namespace Battlehub.RTHandles
{
    public class VRPhysicsRaycastTool : VRMoveXZTool
    {
        [SerializeField]
        private float m_spereCastRadius = 0.15f;

        protected override bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance)
        {
            if(m_spereCastRadius <= 0)
            {
                return Physics.Raycast(origin, direction, out hit, maxDistance);
            }
            return Physics.SphereCast(origin, m_spereCastRadius, direction, out hit, maxDistance);
        }
    }
}

