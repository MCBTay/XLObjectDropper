using UnityEngine;

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

		private void Awake()
		{
			AButton = AButtonGameObject.GetComponent<AXYBButton>();
			XButton = XButtonGameObject.GetComponent<AXYBButton>();
			YButton = YButtonGameObject.GetComponent<AXYBButton>();
			BButton = BButtonGameObject.GetComponent<AXYBButton>();
		}

		private void OnEnable()
		{
			SetDefaultState();
		}

		private void OnDisable()
		{
			SetDefaultState();
		}

		private void Start()
		{
			SetDefaultState();
		}

		private void SetDefaultState()
		{
			AButton.SetDefaultState();
			BButton.SetDefaultState();
			XButton.SetDefaultState();
			YButton.SetDefaultState();
		}

		private void Update()
		{
			HandleButton(AButton, "A");
			HandleButton(BButton, "B");
			HandleButton(XButton, "X");
			HandleButton(YButton, "Y");
		}

		private void HandleButton(AXYBButton button, string inputButton)
		{
			var player = UIManager.Instance.Player;

			if (button.ButtonEnabled)
			{
				if (player.GetButtonDown(inputButton))
					button.ToggleButtonSprite(true);

				if (player.GetButtonUp(inputButton))
					button.ToggleButtonSprite(false);
			}
			else
			{
				button.ToggleButtonSprite(false);
			}
		}
	}
}