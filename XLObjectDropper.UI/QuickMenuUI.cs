using UnityEngine;

namespace XLObjectDropper.UI
{
	public class QuickMenuUI : ObjectSelectionBase<QuickMenuType>
	{
		[Header("Categories")]
		public GameObject PlacedCategory;
		public GameObject RecentCategory;

		private void Awake()
		{
			Categories.Add(QuickMenuType.Placed, PlacedCategory);
			Categories.Add(QuickMenuType.Recent, RecentCategory);
		}
	}
}
