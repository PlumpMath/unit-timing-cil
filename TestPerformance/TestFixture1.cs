using System.Threading;

using UnitPerformance;

namespace TestPerformance
{
	[PerformanceFixture]
	public class TestFixture1
	{
		#region Setup and Teardown

		#endregion

		#region Performance Units

		[Performance(1, 2, 4)]
		public void Test1()
		{
			Thread.Sleep(250);
		}

		#endregion
	}
}
