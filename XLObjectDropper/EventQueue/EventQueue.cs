using System.Collections.Generic;
using XLObjectDropper.EventQueue.Events;

namespace XLObjectDropper.EventQueue
{
	public class EventQueue
	{
		private static EventQueue _instance;
		public static EventQueue Instance => _instance ?? (_instance = new EventQueue());
		
		public Queue<ObjectDropperEvent> UndoQueue { get; set; }
		public Queue<ObjectDropperEvent> RedoQueue { get; set; }

		public EventQueue()
		{
			UndoQueue = new Queue<ObjectDropperEvent>(10);
			RedoQueue = new Queue<ObjectDropperEvent>(10);
		}

		public void AddNewAction(ObjectDropperEvent objectDropperEvent)
		{
			UndoQueue.Enqueue(objectDropperEvent);
		}

		public void UndoAction()
		{
			var objectDropperEvent = UndoQueue.Dequeue();
			objectDropperEvent.Undo();
			RedoQueue.Enqueue(objectDropperEvent);
		}

		public void RedoAction()
		{
			var objectDropperEvent = RedoQueue.Dequeue();
			objectDropperEvent.Redo();
			UndoQueue.Enqueue(objectDropperEvent);
		}
	}
}
