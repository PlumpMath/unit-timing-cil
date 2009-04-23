using System;

namespace UnitPerformance
{
	/// <summary>
	/// Marks the method used to set up the performance units within
	/// a specific fixture. This is called once before anything else
	/// is called in a given fixture.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class PerformanceFixtureSetupAttribute
		: Attribute
	{
	}
}
