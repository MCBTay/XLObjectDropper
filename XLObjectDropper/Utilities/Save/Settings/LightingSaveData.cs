using System;
using UnityEngine.Rendering.HighDefinition;

namespace XLObjectDropper.Utilities.Save.Settings
{
	[Serializable]
	public class LightingSaveData : ISettingsSaveData
	{
		public bool enabled;
		public float intensity;
		public LightUnit unit;
		public float range;
		public float? angle;
		public SerializableVector3 color;
	}
}
