using System;
using System.Threading;

using UnitTiming;

namespace Tests
{
	[TimingFixture]
	public class TestFixture1
	{
		#region Setup and Teardown

		#endregion

		#region Timing Units

		[Timing]
		public void TestDefault0()
		{
		}

		[Timing(1, 2, 3, 8)]
		public void Test0()
		{
			Thread.Sleep(5);
		}

		[Timing(1, 2, 4, 8)]
		public void Test1(int count)
		{
			Console.WriteLine("Count: {0}", count);
		}

		[Timing(1, 2, 4, 8)]
		public void Test2(int count, int iteration)
		{
			Console.WriteLine("Iteration: {0} of {1}", count, iteration);
		}

		[Timing(1, 2, 4, 8, Singleton = true)]
		public void TestSingleton(int iteration)
		{
			Console.WriteLine("Singleton: {0}", iteration);
		}

		#endregion
	}
}