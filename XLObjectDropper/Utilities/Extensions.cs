using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLObjectDropper.Controllers.ObjectEdit;
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

			if (!layerInfo.Enabled)
			{
				//TODO: Add an enum to prevent hardcoding this layer (0)
				trans.gameObject.layer = 0;
			}

			for (int i = trans.childCount - 1; i >= 0; i--)
			{
				trans.GetChild(i).ChangeLayersRecursively(layerInfo.Children.ElementAt(i));
			}
		}

		public static Transform GetTopMostParent(this Transform trans)
		{
			var objectName = trans.gameObject.name.Replace("(Clone)", string.Empty);

			foreach (var spawnable in SpawnableManager.Prefabs)
			{
				if (objectName.Equals(spawnable.Prefab.name))
				{
					return trans;
				}

				var styleSettings = spawnable.Settings.FirstOrDefault(x => x is EditStyleController) as EditStyleController;
				var style = styleSettings?.Styles.FirstOrDefault(x => objectName.Equals(x.Prefab.name));

				if (style != null)
					return trans;
			}

			return trans.parent == null ? null : trans.parent.GetTopMostParent();
		}

		public static LayerInfo GetObjectLayers(this Transform transform, LayerInfo parent = null)
		{
			var layerInfo = new LayerInfo
			{
				GameObjectName = transform.name, 
				Layer = transform.gameObject.layer, 
				Enabled = transform.gameObject.activeSelf, 
				Parent = parent
			};

			foreach (Transform child in transform)
			{
				layerInfo.Children.Add(GetObjectLayers(child, layerInfo));
			}

			return layerInfo;
		}
		#endregion

		public static List<GameObject> GetChildrenOnLayer(this GameObject gameObject, string layerName)
		{
			var results = new List<GameObject>();

			if (gameObject.layer == LayerMask.NameToLayer(layerName))
			{
				results.Add(gameObject);
			}

			foreach (Transform child in gameObject.transform)
			{
				results.AddRange(GetChildrenOnLayer(child.gameObject, layerName));
			}

			return results;
		}
	}
}
