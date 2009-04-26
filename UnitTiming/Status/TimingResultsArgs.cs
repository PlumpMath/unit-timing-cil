using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnitTiming.Status
{
	/// <summary>
	/// Encapsulates all the results from a timing run.
	/// </summary>
	public class TimingResultsArgs
	{
		public TimeSpan BaselineTime
		{
			get; set;
		}

		public MethodInfo Method
		{
			get; set;
		}

		public int Iterations
		{
			get; set;
		}

		public TimeSpan Time
		{
			get; set;
		}
	}
}
