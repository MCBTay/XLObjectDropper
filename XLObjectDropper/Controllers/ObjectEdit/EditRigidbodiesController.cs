using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLObjectDropper.UI.Controls.Expandables;
using XLObjectDropper.UI.Menus;
using XLObjectDropper.Utilities;
using XLObjectDropper.Utilities.Save;
using XLObjectDropper.Utilities.Save.Settings;

namespace XLObjectDropper.Controllers.ObjectEdit
{
	public class EditRigidbodiesController : IObjectSettings
	{
		private static EditRigidbodiesController _instance;
		public static EditRigidbodiesController Instance => _instance ?? (_instance = new EditRigidbodiesController());

		public bool EnablePhysics;
		public bool EnableRespawnRecall;
		public Vector3 RecallPosition;
		public Quaternion RecallRotation;

		public void AddOptions(GameObject SelectedObject, ObjectEditUI ObjectEdit)
		{
			var rigidBodies = SelectedObject.GetPrefab().GetComponentsInChildren<Rigidbody>(true);
			if (rigidBodies == null || !rigidBodies.Any()) return;

			var rigidbodyExpandable = ObjectEdit.AddRigidbodySettings();
			var expandable = rigidbodyExpandable.GetComponent<RigidbodySettingsExpandable>();

			var spawnable = SelectedObject.GetSpawnableFromSpawned() ?? SelectedObject.GetSpawnable();
			if (spawnable == null) return;

			expandable.EnablePhysicsToggle.Toggle.isOn = EnablePhysics;
			expandable.EnablePhysicsToggle.Toggle.interactable = true;
			expandable.EnablePhysicsToggle.Toggle.onValueChanged.AddListener((isOn) =>
			{
				EnablePhysics = isOn;
				ToggleRigidBodies(SelectedObject, isOn);
			});


			expandable.EnableRespawnRecallToggle.Toggle.isOn = EnableRespawnRecall;
			expandable.EnableRespawnRecallToggle.Toggle.interactable = true;
			expandable.EnableRespawnRecallToggle.Toggle.onValueChanged.AddListener((isOn) =>
			{
				EnableRespawnRecall = isOn;
				RecallPosition = EnableRespawnRecall ? SelectedObject.transform.position : Vector3.zero;
				RecallRotation = EnableRespawnRecall ? SelectedObject.transform.rotation : Quaternion.identity;
			});
		}

		private void ToggleRigidBodies(GameObject gameObject, bool toggle)
		{
			var rigidBodies = gameObject.GetComponentsInChildren<Rigidbody>(true);
			if (rigidBodies == null || !rigidBodies.Any()) return;

			foreach (var rigidBody in rigidBodies)
			{
				rigidBody.isKinematic = !toggle;
			}
		}

		public ISettingsSaveData ConvertToSaveSettings()
		{
			return new RigidbodySaveData
			{
				enablePhysics = EnablePhysics,
				enableRespawnRecall = EnableRespawnRecall,
				recallPosition = new SerializableVector3(RecallPosition),
				recallRotation = new SerializableQuaternion(RecallRotation),
			};
		}

		public void ApplySaveSettings(GameObject selectedObject, List<ISettingsSaveData> settings)
		{
			if (settings == null || !settings.Any()) return;

			var rigidbodySettings = settings.OfType<RigidbodySaveData>().ToList();
			if (!rigidbodySettings.Any()) return;

			var rigidbodySetting = rigidbodySettings.First();

			EnablePhysics = rigidbodySetting.enablePhysics;
			EnableRespawnRecall = rigidbodySetting.enableRespawnRecall;
			RecallPosition = rigidbodySetting.recallPosition;
			RecallRotation = rigidbodySetting.recallRotation;

			ToggleRigidBodies(selectedObject, EnablePhysics);
		}
	}
}
