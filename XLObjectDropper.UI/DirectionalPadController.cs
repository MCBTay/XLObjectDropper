using UnityEngine;

namespace XLObjectDropper.UI
{
	public class DirectionalPadController : MonoBehaviour
	{
		[Header("Directional Pad Pressed")]
		public GameObject DpadUp_Pressed;
		public GameObject DpadDown_Pressed;
		public GameObject DpadLeft_Pressed;
		public GameObject DpadRight_Pressed;

		void OnEnable()
		{
			SetDefaultState();
		}

		void OnDisable()
		{
			SetDefaultState();
		}

		void Start()
		{
			SetDefaultState();
		}

		void SetDefaultState()
		{
			DpadUp_Pressed.SetActive(false);
			DpadDown_Pressed.SetActive(false);
			DpadLeft_Pressed.SetActive(false);
			DpadRight_Pressed.SetActive(false);
		}

		void Update()
		{
			var player = UIManager.Instance.Player;

			#region Right
			if (player.GetButtonDown("DPadX"))
			{
				DpadRight_Pressed.SetActive(true);
			}

			if (player.GetButtonUp("DPadX"))
			{
				DpadRight_Pressed.SetActive(false);
			}
			#endregion

			#region Left
			if (player.GetNegativeButtonDown("DPadX"))
			{
				DpadLeft_Pressed.SetActive(true);
			}

			if (player.GetNegativeButtonUp("DPadX"))
			{
				DpadLeft_Pressed.SetActive(false);
			}
			#endregion

			#region Up
			if (player.GetButtonDown("DPadY"))
			{
				DpadUp_Pressed.SetActive(true);
			}

			if (player.GetButtonUp("DPadY"))
			{
				DpadUp_Pressed.SetActive(false);
			}
			#endregion

			#region Down
			if (player.GetNegativeButtonDown("DPadY"))
			{
				DpadDown_Pressed.SetActive(true);
			}

			if (player.GetNegativeButtonUp("DPadY"))
			{
				DpadDown_Pressed.SetActive(false);
			}
			#endregion
		}
	}
}