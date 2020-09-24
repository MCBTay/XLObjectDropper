namespace XLObjectDropper.UI.Utilities
{
	public static class Color
	{
		public static float GetAlpha(bool buttonEnabled) { return buttonEnabled ? 1.0f : 0.3f; }

		public static string ColorTag => "<color=#3286EC>";
	}
}
