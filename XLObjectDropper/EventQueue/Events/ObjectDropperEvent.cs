using UnityEngine;

namespace XLObjectDropper.EventQueue.Events
{
	public abstract class ObjectDropperEvent
	{
		public GameObject GameObject;

		public abstract void Undo();
		public abstract void Redo();

		public ObjectDropperEvent(GameObject gameObject)
		{
			GameObject = gameObject;
		}
	}
}
