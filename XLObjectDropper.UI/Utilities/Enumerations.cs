namespace XLObjectDropper.UI.Utilities
{
	public enum ScalingMode
	{
		Uniform,
		Width,
		Height,
		Depth
	}

	public enum RotationSnappingMode
	{
		Off,
		Degrees15,
		Degrees45,
		Degrees90
	}

	public enum QuickMenuType
	{
		Placed,
		Recent
	}

	public enum MovementSnappingMode
	{
		Off,
		Quarter,
		Half,
		Full,
		Double
	}

	public enum ScaleSnappingMode
	{
		Off,
		Quarter,
		Half,
		Full
	}

	public enum PlatformType
	{
		Xbox,
		Playstation,
		Switch
	}

	/// <summary>
	/// These are in regards to the Rewired controller buttons Easy Day has set up, which are XBox based.
	/// </summary>
	public enum ControllerButton
	{
		A,
		B,
		X,
		Y,
		RT,
		RB,
		LT,
		LB,
		DPadX,
		DPadY,
		Select,
		Start,
		LeftStickX,
		LeftStickY,
		Left_Stick_Button,
		RightStickX,
		RightStickY,
		Right_Stick_Button
	}
}
