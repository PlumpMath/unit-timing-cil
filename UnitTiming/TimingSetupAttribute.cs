using System;

namespace UnitTiming
{
	/// <summary>
	/// Marks the method used to set up a performance unit. For multiples,
	/// this will be called for every round of a given test.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class TimingSetupAttribute
		: Attribute
	{
	}
}