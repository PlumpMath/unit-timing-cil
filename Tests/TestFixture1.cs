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

		[Timing(1, 2, 4, 16, 32)]
		public void Test1()
		{
			Thread.Sleep(250);
		}

		#endregion
	}
}