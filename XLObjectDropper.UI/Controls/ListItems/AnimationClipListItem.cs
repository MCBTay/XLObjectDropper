using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XLObjectDropper.UI.Controls.ListItems
{
	public class AnimationClipListItem : MonoBehaviour
	{
		public GameObject ListItem;
		public TMP_Text ClipName;
		[HideInInspector] public UnityEvent onSelect;

		public void Update()
		{
			GetComponent<Outline>().enabled = false;

			if (ListItem == EventSystem.current.currentSelectedGameObject)
			{
				GetComponent<Outline>().enabled = true;
			}
		}
	}
}
