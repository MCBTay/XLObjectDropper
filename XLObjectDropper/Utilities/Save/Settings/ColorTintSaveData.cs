using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLObjectDropper.Utilities.Save.Settings
{
	[Serializable]
	public class ColorTintSaveData : ISettingsSaveData
	{
		public SerializableVector3 tintedColor;
	}
}
