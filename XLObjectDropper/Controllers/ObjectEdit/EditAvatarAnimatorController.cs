using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using XLObjectDropper.UI.Controls.Expandables;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities.Save.Settings;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public class EditAvatarAnimatorController : IObjectSettings
	{
		private static EditAvatarAnimatorController _instance;
		public static EditAvatarAnimatorController Instance => _instance ?? (_instance = new EditAvatarAnimatorController());

		private Animator Animator;
		private AnimationClip[] Animations;

		private string CurrentAnimation;

		public void AddOptions(GameObject SelectedObject, ObjectEditUI ObjectEdit)
		{
			Animator = SelectedObject.GetComponentInChildren<Animator>();
			if (Animator == null) return;
			if (Animator.runtimeAnimatorController == null) return;
			if (!Animator.runtimeAnimatorController.animationClips.Any()) return;
			if (Animator.avatar == null) return;


			Animations = Animator.runtimeAnimatorController.animationClips;

			var settings = ObjectEdit.AddAvatarAnimatorSettings();

			var expandable = settings.GetComponent<Expandable>();
			var avatarAnimatorExpandable = settings.GetComponent<AvatarAnimatorExpandable>();

			foreach (var animation in Animations)
			{
				AddListItem(avatarAnimatorExpandable.ListItemPrefab, expandable.PropertiesListContent.transform, animation);
			}
		}

		private void AddListItem(GameObject prefab, Transform listContent, AnimationClip animationClip)
		{
			var listItem = Object.Instantiate(prefab, listContent);

			string displayText = animationClip.name;

			if (displayText.StartsWith("Armature|"))
			{
				displayText = displayText.Replace("Armature|", string.Empty);
			}

			listItem.GetComponentInChildren<TMP_Text>().SetText(displayText);

			listItem.GetComponent<Button>().onClick.AddListener(() =>
			{
				CurrentAnimation = animationClip.name;
				Animator.Play(animationClip.name);
			});

			//if (objectSelected != null)
			//{
			//	listItem.GetComponent<ObjectSelectionListItem>().onSelect.AddListener(objectSelected);
			//}

			listItem.SetActive(true);
		}

		public ISettingsSaveData ConvertToSaveSettings()
		{
			return new AvatarAnimatorSaveData
			{
				currentAnimationName = CurrentAnimation,
				isPlaying = true
			};
		}

		public void ApplySaveSettings(GameObject selectedObject, List<ISettingsSaveData> settings)
		{
			if (settings == null || !settings.Any()) return;

			var animationSettings = settings.OfType<AvatarAnimatorSaveData>().ToList();
			if (!animationSettings.Any()) return;

			var animationSetting = animationSettings.First();

			CurrentAnimation = animationSetting.currentAnimationName;
			selectedObject.GetComponentInChildren<Animator>().Play(CurrentAnimation);

			var aimConstraint = selectedObject.GetComponentInChildren<AimConstraint>(true);
			var aimTarget = selectedObject.transform.Find("Target");
			if (aimConstraint != null && aimTarget != null)
			{
				aimTarget.gameObject.AddComponent<AimConstraintTargetController>();
			}
		}
	}
}
