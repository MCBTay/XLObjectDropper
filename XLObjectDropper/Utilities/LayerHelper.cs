using System.Collections.Generic;
using UnityEngine;

namespace XLObjectDropper.Utilities
{
	public class LayerInfo
	{
		public string GameObjectName;
		public int Layer;

		public LayerInfo Parent;
		public List<LayerInfo> Children;

		public LayerInfo()
		{
			GameObjectName = string.Empty;
			Layer = -1;

			Parent = null;
			Children = new List<LayerInfo>();
		}
	}
}
