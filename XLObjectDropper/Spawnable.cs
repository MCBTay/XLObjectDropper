using UnityEngine;

namespace XLObjectDropper
{
	public class Spawnable
	{
		public GameObject Prefab;
		public int OriginalLayer;

		public Spawnable()
		{
			Prefab = null;
			OriginalLayer = -1;
		}
	}
}
