using System;
using System.IO;
using System.Linq;
using Rewired;
using UnityEngine;
using UnityEngine.Events;
using XLObjectDropper.UI;
using XLObjectDropper.UI.Menus;

namespace XLObjectDropper.Controllers
{
	public class SaveController : MonoBehaviour
	{
		public static SaveUI SaveUI;
		public event UnityAction Saved = () => { };
		public event UnityAction SaveCancelled = () => { };

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
			var text = SaveUI.InputField.text;

			if (text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				UISounds.Instance.uiSource.PlayOneShot(UISounds.Instance.FailedTrickSounds.First(), 1.0f);
			}
			else
			{
				UISounds.Instance?.PlayOneShotSelectMajor();
				Utilities.SaveManager.Instance.SaveCurrentSpawnables(text);

				Saved.Invoke();
			}
		}

		private void CancelClicked()
		{
			UISounds.Instance?.PlayOneShotSelectMajor();
			SaveCancelled.Invoke();
		}
	}
}
