using UnityEngine;

namespace XLObjectDropper.Utilities
{
	public static class DebugUtilities
	{
		public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 100f)
		{
			GameObject myLine = new GameObject();
			myLine.transform.position = start;
			myLine.AddComponent<LineRenderer>();
			LineRenderer lr = myLine.GetComponent<LineRenderer>();
			lr.material = new Material(Shader.Find("HDRP/Lit"));
			lr.startColor = lr.endColor = color;
			lr.startWidth = lr.endWidth = 0.1f;
			lr.SetPosition(0, start);
			lr.SetPosition(1, end);
			Object.Destroy(myLine, duration);
		}

		public static void DrawRay(Ray ray, float distance, Color color, float duration = 100f)
		{
			GameObject myLine = new GameObject();
			myLine.transform.position = ray.origin;
			myLine.AddComponent<LineRenderer>();
			LineRenderer lr = myLine.GetComponent<LineRenderer>();
			lr.material = new Material(Shader.Find("HDRP/Lit"));
			lr.startColor = lr.endColor = color;
			lr.startWidth = lr.endWidth = 0.1f;
			lr.SetPosition(0, ray.origin);
			lr.SetPosition(1, ray.origin + (ray.direction * distance));
			Object.Destroy(myLine, duration);
		}
	}
}
