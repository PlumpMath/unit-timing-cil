using System;

namespace UnitTiming
{
	/// <summary>
	/// Marks the method as producing the baseline for the fixture.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class TimingBaselineAttribute
		: Attribute
	{
	}
}