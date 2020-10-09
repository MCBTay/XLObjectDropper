using System;

namespace XLObjectDropper.Utilities.Save.Settings
{
	[Serializable]
	public class GrindableSaveData : ISettingsSaveData
	{
		public bool grindablesEnabled;
		public bool copingEnabled;
	}
}
