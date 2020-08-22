using UnityEngine;
using UnityEngine.EventSystems;
using UnityModManagerNet;

namespace XLObjectDropper.UI
{
	public class ObjectSelectionListItem : MonoBehaviour, ISelectHandler
	{
		public void OnSelect(BaseEventData eventData)
		{
			UnityModManager.Logger.Log(this.gameObject.name + " was selected");
		}
	}
}
