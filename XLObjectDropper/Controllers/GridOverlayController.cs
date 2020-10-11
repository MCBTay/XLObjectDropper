using UnityEngine;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.Controllers
{
	public class GridOverlayController : MonoBehaviour
	{
		public GameObject GridOverlay;
		private bool GridOverlayActive => GridOverlay != null && GridOverlay.activeInHierarchy;

		private void OnEnable()
		{
			GridOverlay = Instantiate(AssetBundleHelper.GridOverlayPrefab);
			GridOverlay.transform.position = ObjectMovementController.Instance.transform.position;
		}

		private void Update()
		{
			GridOverlay.transform.position = ObjectMovementController.Instance.transform.position;
		}

		private void OnDisable()
		{
			if (GridOverlay != null)
			{
				GridOverlay.SetActive(false);
				DestroyImmediate(GridOverlay);
			}
		}
	}
}
