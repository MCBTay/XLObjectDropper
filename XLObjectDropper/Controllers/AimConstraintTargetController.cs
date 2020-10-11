using GameManagement;
using ReplayEditor;
using UnityEngine;

namespace XLObjectDropper.Controllers
{
	public class AimConstraintTargetController : MonoBehaviour
	{
		private void Update()
		{
			if (GameStateMachine.Instance.CurrentState.GetType() == typeof(ReplayState))
			{
				gameObject.transform.position = ReplayEditorController.Instance.transform.Find("Playback Skater Root/NewSkater/Skater_Joints/Skater_root/Skater_pelvis").position;
			}
			else
			{
				gameObject.transform.position = PlayerController.Instance.skaterController.skaterTargetTransform.position;
			}
		}
	}
}
