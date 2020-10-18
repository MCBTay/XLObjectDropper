using System;

namespace XLObjectDropper.Utilities.Save.Settings
{
	[Serializable]
	public class AvatarAnimatorSaveData : ISettingsSaveData
	{
		public string currentAnimationName;
		public bool isPlaying;
	}
}
