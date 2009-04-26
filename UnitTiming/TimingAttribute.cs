using System;

namespace UnitTiming
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
	public class TimingAttribute
		: Attribute
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TimingAttribute"/> class.
		/// </summary>
		/// <param name="iterations">The runs.</param>
		public TimingAttribute(params int[] iterations)
		{
			if (iterations == null || iterations.Length == 0)
			{
				this.iterations = new int[] { 1 };
			}
			else
			{
				this.iterations = iterations;
			}
		}

		#endregion Constructors

		#region Fields

		private readonly int[] iterations;

		/// <summary>
		/// Gets the iterations for this run.
		/// </summary>
		/// <value>The iterations.</value>
		public int[] Iterations
		{
			get { return iterations; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this test should only be run once, with
		/// a given signature.
		/// </summary>
		/// <value><c>true</c> if singleton; otherwise, <c>false</c>.</value>
		public bool Singleton { get; set; }

		#endregion
	}
}