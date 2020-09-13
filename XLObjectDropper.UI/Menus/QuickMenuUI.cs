using UnityEngine;
using XLObjectDropper.UI.Utilities;

namespace XLObjectDropper.UI.Menus
{
	public class QuickMenuUI : ObjectSelectionBase<QuickMenuType>
	{
		[Header("Categories")]
		public GameObject PlacedCategory;
		public GameObject RecentCategory;

		protected override void Awake()
		{
			base.Awake();

			Categories.Add(QuickMenuType.Placed, PlacedCategory);
			Categories.Add(QuickMenuType.Recent, RecentCategory);
		}
	}
}
