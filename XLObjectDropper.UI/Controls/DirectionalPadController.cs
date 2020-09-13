using TMPro;
using UnityEngine;

namespace XLObjectDropper.UI.Controls
{
	public class DirectionalPadController : MonoBehaviour
	{
		[Header("Labels")]
		public TMP_Text UpLabel;
		public TMP_Text DownLabel;
		public TMP_Text LeftLabel;
		public TMP_Text RightLabel;

		[Header("Directional Pad Pressed")]
		public GameObject UpPressed;
		public GameObject DownPressed;
		public GameObject LeftPressed;
		public GameObject RightPressed;

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
			UpPressed.SetActive(false);
			DownPressed.SetActive(false);
			LeftPressed.SetActive(false);
			RightPressed.SetActive(false);
		}

		private bool LockCam;

		void Update()
		{
			var player = UIManager.Instance.Player;

			#region Right
			if (player.GetButtonDown("DPadX"))
			{
				RightPressed.SetActive(true);
				LockCam = !LockCam;

				if (!player.GetButton("LB"))
				{
					RightLabel.SetText($"LOCK CAM: <color=#3286EC>{(LockCam ? "ON" : "OFF")}");
				}
			}

			if (player.GetButtonUp("DPadX"))
			{
				RightPressed.SetActive(false);
			}
			#endregion

			#region Left
			if (player.GetNegativeButtonDown("DPadX"))
			{
				LeftPressed.SetActive(true);
			}

			if (player.GetNegativeButtonUp("DPadX"))
			{
				LeftPressed.SetActive(false);
			}
			#endregion

			#region Up
			if (player.GetButtonDown("DPadY"))
			{
				UpPressed.SetActive(true);
			}

			if (player.GetButtonUp("DPadY"))
			{
				UpPressed.SetActive(false);
			}
			#endregion

			#region Down
			if (player.GetNegativeButtonDown("DPadY"))
			{
				DownPressed.SetActive(true);
			}

			if (player.GetNegativeButtonUp("DPadY"))
			{
				DownPressed.SetActive(false);
			}
			#endregion
		}
	}
}