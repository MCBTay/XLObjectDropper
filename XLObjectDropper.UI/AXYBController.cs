using UnityEngine;

namespace XLObjectDropper.UI
{
	public class AXYBController : MonoBehaviour
	{
		[Header("A, X, Y, B Buttons")] 
		public GameObject AButton;
		public GameObject XButton;
		public GameObject YButton;
		public GameObject BButton;

		[Space(10)] 
		[Header("A, X, Y, B Buttons Pressed")]
		public GameObject AButton_Pressed;
		public GameObject XButton_Pressed;
		public GameObject YButton_Pressed;
		public GameObject BButton_Pressed;

		void OnEnable()
		{
			SetDefaultState();
		}

		void OnDisable()
		{
			SetDefaultState();
		}

		void SetDefaultState()
		{
			AButton.SetActive(true);
			XButton.SetActive(true);
			YButton.SetActive(true);
			BButton.SetActive(true);

			AButton_Pressed.SetActive(false);
			XButton_Pressed.SetActive(false);
			YButton_Pressed.SetActive(false);
			BButton_Pressed.SetActive(false);
		}

		void Start()
		{
			SetDefaultState();
		}

		// Update is called once per frame
		void Update()
		{
			var player = UIManager.Instance.Player;

			#region A

			if (player.GetButtonDown("A"))
			{
				AButton.SetActive(false);
				AButton_Pressed.SetActive(true);
			}

			if (player.GetButtonUp("A"))
			{
				AButton.SetActive(true);
				AButton_Pressed.SetActive(false);
			}

			#endregion

			#region X

			if (player.GetButtonDown("X"))
			{
				XButton.SetActive(false);
				XButton_Pressed.SetActive(true);
			}

			if (player.GetButtonUp("X"))
			{
				XButton.SetActive(true);
				XButton_Pressed.SetActive(false);
			}

			#endregion

			#region Y

			if (player.GetButtonDown("Y"))
			{
				YButton.SetActive(false);
				YButton_Pressed.SetActive(true);
			}

			if (player.GetButtonUp("Y"))
			{
				YButton.SetActive(true);
				YButton_Pressed.SetActive(false);
			}

			#endregion

			#region B

			if (player.GetButtonDown("B"))
			{
				BButton.SetActive(false);
				BButton_Pressed.SetActive(true);
			}

			if (player.GetButtonUp("B"))
			{
				BButton.SetActive(true);
				BButton_Pressed.SetActive(false);
			}

			#endregion
		}
	}
}
