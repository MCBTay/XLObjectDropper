using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace XLObjectDropper.UI.Controls
{
	public class SelectableControl : MonoBehaviour, ISelectHandler
	{
		[HideInInspector] public event UnityAction onSelect = () => { };

		public void OnSelect(BaseEventData eventData)
		{
			onSelect.Invoke();
		}
	}
}
