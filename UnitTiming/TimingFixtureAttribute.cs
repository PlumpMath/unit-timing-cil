using System;

namespace UnitTiming
{
	/// <summary>
	/// Defines a class that has performance units contained inside.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class TimingFixtureAttribute
		: Attribute
	{
	}
}