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

		void Update()
		{
			UpdateDPadSprite(RightPressed, "DPadX", false);
			UpdateDPadSprite(LeftPressed, "DPadX", true);

			UpdateDPadSprite(UpPressed, "DPadY", false);
			UpdateDPadSprite(DownPressed, "DPadY", true);
		}

		private void UpdateDPadSprite(GameObject gameObject, string dpadToUpdate, bool isNegative)
		{
			var player = UIManager.Instance.Player;

			if (isNegative)
			{
				if (player.GetNegativeButtonDown(dpadToUpdate)) gameObject.SetActive(true);
				if (player.GetNegativeButtonUp(dpadToUpdate)) gameObject.SetActive(false);
			}
			else
			{
				if (player.GetButtonDown(dpadToUpdate)) gameObject.SetActive(true);
				if (player.GetButtonUp(dpadToUpdate)) gameObject.SetActive(false);
			}
		}
	}
}