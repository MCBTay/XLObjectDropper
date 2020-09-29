using UnityEngine;
using XLObjectDropper.SpawnableScripts;
using XLObjectDropper.UI.Utilities;

namespace XLObjectDropper.UI.Menus
{
	public class ObjectSelectionUI : ObjectSelectionBase<Enumerations.SpawnableType>
	{
		[Header("Categories")]
		public GameObject RailsCategory;
		public GameObject RampsCategory;
		public GameObject SplinesCategory;
		public GameObject PropsCategory;
		public GameObject ParkCategory;
		public GameObject PacksCategory;

		protected override void Awake()
		{
			base.Awake();

			Categories.Add(Enumerations.SpawnableType.Rails, RailsCategory);
			Categories.Add(Enumerations.SpawnableType.Ramps, RampsCategory);
			Categories.Add(Enumerations.SpawnableType.Splines, SplinesCategory);
			Categories.Add(Enumerations.SpawnableType.Props, PropsCategory);
			Categories.Add(Enumerations.SpawnableType.Park, ParkCategory);
			Categories.Add(Enumerations.SpawnableType.Packs, PacksCategory);
		}
	}
}
