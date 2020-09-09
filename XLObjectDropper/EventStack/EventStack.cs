using System.Collections.Generic;
using XLObjectDropper.EventStack.Events;

namespace XLObjectDropper.EventStack
{
	public class EventStack
	{
		private static EventStack _instance;
		public static EventStack Instance => _instance ?? (_instance = new EventStack());
		
		public Stack<ObjectDropperEvent> UndoQueue { get; set; }
		public Stack<ObjectDropperEvent> RedoQueue { get; set; }

		public EventStack()
		{
			UndoQueue = new Stack<ObjectDropperEvent>(10);
			RedoQueue = new Stack<ObjectDropperEvent>(10);
		}

		public void AddNewAction(ObjectDropperEvent objectDropperEvent)
		{
			UndoQueue.Push(objectDropperEvent);
		}

		public void UndoAction()
		{
			var objectDropperEvent = UndoQueue.Pop();
			objectDropperEvent.Undo();
			RedoQueue.Push(objectDropperEvent);
		}

		public void RedoAction()
		{
			var objectDropperEvent = RedoQueue.Pop();
			objectDropperEvent.Redo();
			UndoQueue.Push(objectDropperEvent);
		}
	}
}
