using System.Linq;
using UnityEngine;
using XLObjectDropper.Utilities;

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

		public static void ChangeLayersRecursively(this Transform trans, LayerInfo layerInfo)
		{
			trans.gameObject.layer = layerInfo.Layer;

			for (int i = trans.childCount - 1; i >= 0; i--)
			{
				trans.GetChild(i).ChangeLayersRecursively(layerInfo.Children.ElementAt(i));
			}
		}

		public static Transform GetTopMostParent(this Transform trans)
		{
			var objectName = trans.gameObject.name.Replace("(Clone)", string.Empty);

			var prefab = SpawnableManager.Prefabs.FirstOrDefault(x => objectName.Equals(x.Prefab.name));
			if (prefab != null)
			{
				return trans;
			}

			return trans.parent == null ? null : trans.parent.GetTopMostParent();
		}

		public static LayerInfo GetObjectLayers(this Transform transform, LayerInfo parent = null)
		{
			var layerInfo = new LayerInfo { GameObjectName = transform.name, Layer = transform.gameObject.layer, Parent = parent };

			foreach (Transform child in transform)
			{
				layerInfo.Children.Add(GetObjectLayers(child, layerInfo));
			}

			return layerInfo;
		}
		#endregion

		#region GameObject extensions
		public static GameObject GetPrefab(this GameObject gameObject)
		{
			var name = gameObject.name.Replace("(Clone)", string.Empty);

			return SpawnableManager.Prefabs.FirstOrDefault(x => name.Equals(x.Prefab.name))?.Prefab;
		}
		#endregion
	}
}
