using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XLObjectDropper.Utilities;

namespace XLObjectDropper.EventStack.Events
{
	public class BatchObjectDeletedEvent : ObjectDropperEvent
	{
		private List<ObjectDeletedEvent> Events;

		public BatchObjectDeletedEvent(List<GameObject> gameObjects) : base(gameObjects)
		{
			Events = new List<ObjectDeletedEvent>();

			foreach (var gameObject in GameObjects)
			{
				var objDeletedEvent = new ObjectDeletedEvent(gameObject.GetPrefab(), gameObject);
				Events.Add(objDeletedEvent);
			}
		}

		public override void Undo()
		{
			if (Events == null || !Events.Any()) return;

			foreach (var deletedEvent in Events)
			{
				deletedEvent.Undo();
			}
		}

		public override void Redo()
		{
			if (Events == null || !Events.Any()) return;

			foreach (var deletedEvent in Events)
			{
				deletedEvent.Redo();
			}
		}
	}
}
