using UnityEngine;

namespace XLObjectDropper.EventStack.Events
{
	public class ObjectPlacedEvent : ObjectDropperEvent
	{
		private GameObject prefab;
		private Vector3 position;
		private Quaternion rotation;
		private Vector3 localScale;

		public override void Undo()
		{
			GameObject.SetActive(false);
			Object.DestroyImmediate(GameObject);
		}

		public override void Redo()
		{
			GameObject = Object.Instantiate(prefab, position, rotation);
			GameObject.transform.localScale = localScale;
			GameObject.SetActive(true);
		}

		public ObjectPlacedEvent(GameObject prefab, GameObject gameObject) : base(gameObject)
		{
			this.prefab = prefab;
			position = gameObject.transform.position;
			rotation = gameObject.transform.rotation;
			localScale = gameObject.transform.localScale;
		}
	}
}
