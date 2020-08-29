using UnityEngine;

namespace XLObjectDropper
{
	public static class Extensions
	{
		#region Transform extensions
		public static void ChangeLayersRecursively(this Transform trans, string name)
		{
			trans.gameObject.layer = LayerMask.NameToLayer(name);
			foreach (Transform child in trans)
			{
				child.ChangeLayersRecursively(name);
			}
		}

		public static void ChangeLayersRecursively(this Transform trans, int layer)
		{
			trans.gameObject.layer = layer;
			foreach (Transform child in trans)
			{
				child.ChangeLayersRecursively(layer);
			}
		}

		public static void ChangeLayersRecursively(this Transform trans, GameObject prefab)
		{
			trans.gameObject.layer = prefab.layer;

			for (var i = trans.childCount - 1; i >= 0; i--)
			{
				trans.GetChild(i).ChangeLayersRecursively(prefab.transform.GetChild(i).gameObject);
			}
		}
		#endregion
	}
}
