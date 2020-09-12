using UnityEngine;

namespace XLObjectDropper.EventStack.Events
{
	public class ObjectDeletedEvent : ObjectDropperEvent
	{
		private GameObject prefab;
		private Vector3 position;
		private Quaternion rotation;
		private Vector3 localScale;

		public override void Undo()
		{
			GameObject = Object.Instantiate(prefab, position, rotation);
			GameObject.transform.localScale = localScale;
			GameObject.SetActive(true);
		}

		public override void Redo()
		{
			GameObject.SetActive(false);
			Object.DestroyImmediate(GameObject);
		}

		public ObjectDeletedEvent(GameObject prefab, GameObject gameObject) : base(gameObject)
		{
			this.prefab = prefab;
			position = gameObject.transform.position;
			rotation = gameObject.transform.rotation;
			localScale = gameObject.transform.localScale;
		}
	}
}
