using System;
using UnityModManagerNet;
using XLObjectDropper.UI.Utilities;

namespace XLObjectDropper
{
	[Serializable]
	public class Settings : UnityModManager.ModSettings
	{
		public static UnityModManager.ModEntry ModEntry;

		public float Sensitivity { get; set; } = 0.5f;
		public bool InvertCamControl { get; set; }
		public bool ShowGrid { get; set; }

		public ScalingMode ScalingMode { get; set; }
		public RotationSnappingMode RotationSnappingMode { get; set; }
		public ScaleSnappingMode ScaleSnappingMode { get; set; }
		public MovementSnappingMode MovementSnappingMode { get; set; }
		public bool GroundTracking { get; set; }

		public static Settings Instance { get; set; }

		public Settings() : base()
		{
		}

		public override void Save(UnityModManager.ModEntry modEntry)
		{
			Save(this, modEntry);
		}

		public void Save()
		{
			Save(ModEntry);
		}
	}
}
