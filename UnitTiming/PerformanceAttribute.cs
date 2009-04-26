using System;

namespace UnitPerformance
{
	/// <summary>
	/// Defines a method which contains a performance unit test. This needs
	/// to have one of three signatures:
	/// 
	/// <list type="bullet">
	/// <item>void MethodName()</item>
	/// <item>void MethodName(int count)</item>
	/// <item>void MethodName(int count, int total)</item>
	/// </list>
	/// 
	/// The attribute takes zero or more integers, which are the number of tests
	/// to run. If none is given, this defaults to 1. Negatives and zeros are
	/// replaced with a single test run.
	/// 
	/// The accessiblity of the test is not important, but are typically public.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class PerformanceAttribute
		: Attribute
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="PerformanceAttribute"/> class.
		/// </summary>
		/// <param name="iterations">The runs.</param>
		public PerformanceAttribute(params int[] iterations)
		{
			this.iterations = iterations;
		}

		#endregion Constructors

		#region Fields

		private readonly int [] iterations;

		/// <summary>
		/// Gets the iterations for this run.
		/// </summary>
		/// <value>The iterations.</value>
		public int [] Iterations
		{
			get { return iterations; }
		}

		#endregion
	}
}
