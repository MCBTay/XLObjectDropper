using UnityEngine;
using UnityEngine.UI;
using XLObjectDropper.UI.Controls.Buttons;

namespace XLObjectDropper.UI.Controls
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

		//[Header("PlayStation Controller Sprites")]
		//public Sprite PlaystationX;
		//public Sprite PlaystationXPressed;
		//public Sprite PlaystationSquare;
		//public Sprite PlaystationSquarePressed;
		//public Sprite PlaystationTriangle;
		//public Sprite PlaystationTrianglePressed;
		//public Sprite PlaystationCircle;
		//public Sprite PlaystationCirclePressed;

		//[Header("Xbox Controller Sprites")]
		//public Sprite XboxX;
		//public Sprite XboxXPressed;
		//public Sprite XboxY;
		//public Sprite XboxYPressed;
		//public Sprite XboxA;
		//public Sprite XboxAPressed;
		//public Sprite XboxB;
		//public Sprite XboxBPressed;

		//[Header("Switch Controller Sprites")]
		//public Sprite SwitchX;
		//public Sprite SwitchXPressed;
		//public Sprite SwitchY;
		//public Sprite SwitchYPressed;
		//public Sprite SwitchB;
		//public Sprite SwitchBPressed;
		//public Sprite SwitchA;
		//public Sprite SwitchAPressed;


		private void Awake()
		{
			AButton = AButtonGameObject.GetComponent<AXYBButton>();
			XButton = XButtonGameObject.GetComponent<AXYBButton>();
			YButton = YButtonGameObject.GetComponent<AXYBButton>();
			BButton = BButtonGameObject.GetComponent<AXYBButton>();
		}
	}
}