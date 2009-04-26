using System;

namespace UnitPerformance
{
	/// <summary>
	/// Marks the method as producing the baseline for the fixture.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class PerformanceBaselineAttribute
		: Attribute
	{
	}
}
