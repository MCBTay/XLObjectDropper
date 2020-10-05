using UnityEngine;
using UnityEngine.UI;
using XLObjectDropper.UI.Controls.Buttons;

namespace XLObjectDropper.UI.Controls.ControllerButtons
{
	public class AXYBController : MonoBehaviour
	{
		[Header("A, X, Y, B Buttons")] 
		public GameObject AButtonGameObject;
		[HideInInspector] public AXYBButton AButton;

		public GameObject XButtonGameObject;
		[HideInInspector] public AXYBButton XButton;

		public GameObject YButtonGameObject;
		[HideInInspector] public AXYBButton YButton;

		public GameObject BButtonGameObject;
		[HideInInspector] public AXYBButton BButton;

		private void Awake()
		{
			AButton = AButtonGameObject.GetComponent<AXYBButton>();
			XButton = XButtonGameObject.GetComponent<AXYBButton>();
			YButton = YButtonGameObject.GetComponent<AXYBButton>();
			BButton = BButtonGameObject.GetComponent<AXYBButton>();
		}
	}
}