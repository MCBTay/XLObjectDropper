using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines.Primitives
{
    public class Capsule : SplinePrimitive
    {
        public float radius = 1f;
        public float height = 2f;

        public override Spline.Type GetSplineType()
        {
            return Spline.Type.Bezier;
        }

        protected override void Generate()
        {
            base.Generate();
            closed = true;
            CreatePoints(7, SplinePoint.Type.SmoothMirrored);
            points[0].position = Vector3.right / 2f * radius + Vector3.forward * height * 0.5f;
            points[0].SetTangentPosition(points[0].position + Vector3.back * 2 * (Mathf.Sqrt(2f) - 1f) / 3f * radius);
            points[1].position = Vector3.forward / 2f * radius + Vector3.forward * height * 0.5f;
            points[1].SetTangentPosition(points[1].position + Vector3.right * 2 * (Mathf.Sqrt(2f) - 1f) / 3f * radius);
            points[2].position = Vector3.left / 2f * radius + Vector3.forward * height * 0.5f;
            points[2].SetTangentPosition(points[2].position + Vector3.forward * 2 * (Mathf.Sqrt(2f) - 1f) / 3f * radius);
            points[3].position = Vector3.left / 2f * radius + Vector3.back * height * 0.5f;
            points[3].SetTangentPosition(points[3].position + Vector3.forward * 2 * (Mathf.Sqrt(2f) - 1f) / 3f * radius);
            points[4].position = Vector3.back / 2f * radius + Vector3.back * height * 0.5f;
            points[4].SetTangentPosition(points[4].position + Vector3.left * 2 * (Mathf.Sqrt(2f) - 1f) / 3f * radius);
            points[5].position = Vector3.right / 2f * radius + Vector3.back * height * 0.5f;
            points[5].SetTangentPosition(points[5].position + Vector3.back * 2 * (Mathf.Sqrt(2f) - 1f) / 3f * radius);
            points[6] = points[0];
        }
    }
}