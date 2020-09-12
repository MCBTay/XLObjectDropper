using UnityEngine;
using UnityModManagerNet;

namespace XLObjectDropper.EventStack.Events
{
	public class ObjectScaleAndRotateEvent : ObjectDropperEvent
	{
		private Quaternion originalRotation;
		public Quaternion newRotation;

		private Vector3 originalLocalScale;
		public Vector3 newLocalScale;

		public override void Undo()
		{
			GameObject.transform.rotation = originalRotation;
			GameObject.transform.localScale = originalLocalScale;
		}

		public override void Redo()
		{
			GameObject.transform.rotation = newRotation;
			GameObject.transform.localScale = newLocalScale;
		}

		public ObjectScaleAndRotateEvent(GameObject gameObject) : base(gameObject)
		{
			originalRotation = gameObject.transform.rotation;
			originalLocalScale = gameObject.transform.localScale;
		}

		public override void AddToUndoStack()
		{
			if (originalRotation != newRotation || originalLocalScale != newLocalScale)
			{
				base.AddToUndoStack();
			}
		}
	}
}
