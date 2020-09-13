using UnityEngine;

namespace XLObjectDropper.UI
{
	public class ObjectSelectionUI : ObjectSelectionBase<SpawnableType>
	{
		[Header("Categories")]
		public GameObject RailsCategory;
		public GameObject RampsCategory;
		public GameObject SplinesCategory;
		public GameObject PropsCategory;
		public GameObject ParkCategory;
		public GameObject PacksCategory;

		
		private void Awake()
		{
			Categories.Add(SpawnableType.Rails, RailsCategory);
			Categories.Add(SpawnableType.Ramps, RampsCategory);
			Categories.Add(SpawnableType.Splines, SplinesCategory);
			Categories.Add(SpawnableType.Props, PropsCategory);
			Categories.Add(SpawnableType.Park, ParkCategory);
			Categories.Add(SpawnableType.Packs, PacksCategory);
		}
	}
}
