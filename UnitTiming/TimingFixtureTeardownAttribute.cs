using System;

namespace UnitTiming
{
	/// <summary>
	/// Marks the methods for taking down the entire fixture. This is
	/// called after all the performance units are run.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class TimingFixtureTeardownAttribute
		: Attribute
	{
	}
}