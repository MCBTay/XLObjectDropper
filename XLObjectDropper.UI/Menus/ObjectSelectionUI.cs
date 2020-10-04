using UnityEngine;
using XLObjectDropper.SpawnableScripts;
using XLObjectDropper.UI.Utilities;

namespace XLObjectDropper.UI.Menus
{
	public class ObjectSelectionUI : ObjectSelectionBase<Enumerations.SpawnableType>
	{
		[Header("Categories")]
		public GameObject RailsAndLedgesCategory;
		public GameObject StairsAndRampsCategory;
		public GameObject FloorsAndWallsCategory;
		public GameObject PropsAndLightsCategory;
		public GameObject BuildingsAndVegetationCategory;
		public GameObject GrindsCategory;
		public GameObject OtherCategory;

		protected override void Awake()
		{
			base.Awake();

			Categories.Add(Enumerations.SpawnableType.RailsAndLedges, RailsAndLedgesCategory);
			Categories.Add(Enumerations.SpawnableType.StairsAndRamps, StairsAndRampsCategory);
			Categories.Add(Enumerations.SpawnableType.FloorsAndWalls, FloorsAndWallsCategory);
			Categories.Add(Enumerations.SpawnableType.PropsAndLights, PropsAndLightsCategory);
			Categories.Add(Enumerations.SpawnableType.BuildingsAndVegetation, BuildingsAndVegetationCategory);
			Categories.Add(Enumerations.SpawnableType.Grinds, GrindsCategory);
			Categories.Add(Enumerations.SpawnableType.Other, OtherCategory);
		}
	}
}
