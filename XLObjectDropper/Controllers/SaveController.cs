using System;
using System.IO;
using System.Linq;
using UnityEngine;
using XLObjectDropper.UI.Menus;

namespace XLObjectDropper.Controllers
{
	public class SaveController : MonoBehaviour
	{
		public static SaveUI SaveUI;

		private void OnEnable()
		{
			SaveUI.InputField.text = $"{LevelManager.Instance.currentLevel.name}_{DateTime.Now:MM.dd.yyyyThh.mm.ss}";
			SaveUI.SaveButton.onClick.AddListener(SaveClicked);
			SaveUI.CancelButton.onClick.AddListener(CancelClicked);
		}

		private void OnDisable()
		{
			SaveUI.SaveButton.onClick.RemoveListener(SaveClicked);
			SaveUI.CancelButton.onClick.RemoveListener(CancelClicked);
		}

		private void SaveClicked()
		{
			//validate text
			var text = SaveUI.InputField.text;

			if (text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				UISounds.Instance.uiSource.PlayOneShot(UISounds.Instance.FailedTrickSounds.First(), 1.0f);
			}
			else
			{
				UISounds.Instance?.PlayOneShotSelectMajor();
				Utilities.SaveManager.Instance.SaveCurrentSpawnables(text);

				gameObject.SetActive(false);
			}
		}

		private void CancelClicked()
		{
			UISounds.Instance?.PlayOneShotSelectMajor();
			gameObject.SetActive(false);
		}
	}
}
