using System;
using UnityModManagerNet;

namespace XLObjectDropper
{
	[Serializable]
	public class Settings : UnityModManager.ModSettings
	{
		public float Sensitivity { get; set; }
		public bool Snapping { get; set; }

		public static Settings Instance { get; set; }

		public Settings() : base()
		{
		}

		public override void Save(UnityModManager.ModEntry modEntry)
		{
			Save(this, modEntry);
		}
	}
}
