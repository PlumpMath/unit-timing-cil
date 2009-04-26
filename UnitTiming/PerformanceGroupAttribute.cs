using System;

namespace UnitPerformance
{
	/// <summary>
	/// Defines a common performance group. This is used for the output
	/// XML and lets the viewing program group related tests. In general,
	/// tests with the same performance group should have the same number
	/// of runs.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class PerformanceGroupAttribute
		: Attribute
	{
	}
}
