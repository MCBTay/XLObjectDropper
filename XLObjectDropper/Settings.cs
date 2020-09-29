using System;
using UnityModManagerNet;
using XLObjectDropper.UI.Utilities;

namespace XLObjectDropper
{
	[Serializable]
	public class Settings : UnityModManager.ModSettings
	{
		public float Sensitivity { get; set; }
		public bool InvertCamControl { get; set; }
		public bool ShowGrid { get; set; }

		public ScalingMode ScalingMode { get; set; }
		public RotationSnappingMode RotationSnappingMode { get; set; }
		public ScaleSnappingMode ScaleSnappingMode { get; set; }
		public MovementSnappingMode MovementSnappingMode { get; set; }

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
