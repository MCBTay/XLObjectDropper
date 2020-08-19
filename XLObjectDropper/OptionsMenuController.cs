using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityModManagerNet;
using XLObjectDropper.UI;
using XLObjectDropper.UserInterface;

namespace XLObjectDropper
{
	public class OptionsMenuController : MonoBehaviour
	{
		public static OptionsMenuUI OptionsMenu { get; set; }

		private void Awake()
		{
			
		}

		private void OnEnable()
		{
			//ObjectMovementController.Instance.enabled = false;

			if (OptionsMenu != null)
			{
				OptionsMenu.SnappingValueChanged += SnappingValueChanged;
				OptionsMenu.SensitivityValueChanged += SensitivityValueChanged;
				OptionsMenu.UndoClicked += UndoClicked;
				OptionsMenu.RedoClicked += RedoClicked;
				OptionsMenu.SaveClicked += SaveClicked;
				OptionsMenu.LoadClicked += LoadClicked;

				OptionsMenu.gameObject.SetActive(true);

				EventSystem.current.SetSelectedGameObject(OptionsMenu.Snapping);
			}
		}

		private void OnDisable()
		{
			//ObjectMovementController.Instance.enabled = true;

			Time.timeScale = 1.0f;

			if (OptionsMenu != null)
			{
				OptionsMenu.SnappingValueChanged -= SnappingValueChanged;
				OptionsMenu.SensitivityValueChanged -= SensitivityValueChanged;
				OptionsMenu.UndoClicked -= UndoClicked;
				OptionsMenu.RedoClicked -= RedoClicked;
				OptionsMenu.SaveClicked -= SaveClicked;
				OptionsMenu.LoadClicked -= LoadClicked;

				OptionsMenu.gameObject.SetActive(false);
			}
		}

		private void Update()
		{

		}

		private static void SnappingValueChanged(bool value)
		{
			UnityModManager.Logger.Log("XLObjectDropper.UserInterfaceHelper: Snapping Value Changed - " + value);
		}

		private static void SensitivityValueChanged(float value)
		{
			UnityModManager.Logger.Log("XLObjectDropper.UserInterfaceHelper: Sensitivity Value Changed - " + value);
		}

		private static void UndoClicked()
		{
			UnityModManager.Logger.Log("XLObjectDropper.UserInterfaceHelper: Undo clicked.");
		}

		private static void RedoClicked()
		{
			UnityModManager.Logger.Log("XLObjectDropper.UserInterfaceHelper: Redo clicked.");
		}

		private static void SaveClicked()
		{
			UnityModManager.Logger.Log("XLObjectDropper.UserInterfaceHelper: Save clicked.");
		}

		private static void LoadClicked()
		{
			UnityModManager.Logger.Log("XLObjectDropper.UserInterfaceHelper: Load clicked.");
		}
	}
}
