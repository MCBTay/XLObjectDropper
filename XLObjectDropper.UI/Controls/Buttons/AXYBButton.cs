namespace XLObjectDropper.UI.Controls.Buttons
{
	public class AXYBButton : ButtonBase
	{
		public void UpdateButton(string text, bool buttonEnabled = true)
		{
			if (!string.IsNullOrEmpty(text))
				ButtonLabel.SetText(text);

			EnableButton(buttonEnabled);
		}
	}
}
