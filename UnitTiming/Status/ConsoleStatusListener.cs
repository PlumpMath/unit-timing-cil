using System;
using System.Reflection;

namespace UnitTiming.Status
{
	/// <summary>
	/// Implements a status listener that gives feedback on the operation
	/// of the performance. In effect, this is just a console output
	/// of key points in the application.
	/// </summary>
	public class ConsoleStatusListener
		: IStatusListener
	{
		#region Status Events
		#endregion Status Events

		#region General Messages

		public void Error(string message, params object[] args)
		{
			Console.WriteLine("Error: " + message, args);
		}

		#endregion

		#region Type Loading

		public void StartLoadingType(Type type)
		{
			Console.WriteLine(" Info: Loading type: {0}", type);
		}

		public void EndLoadingType(Type type)
		{
			Console.WriteLine("Debug: Finished loading type: {0}", type);
		}

		#endregion

		#region Timing

		/// <summary>
		/// Reports that a timing method was found.
		/// </summary>
		/// <param name="methodRunner">The method which is being added.</param>
		public void AddTimingMethod(MethodRunner methodRunner)
		{
			if (methodRunner.TimingAttribute.Singleton)
			{
				Console.WriteLine(" Info:   Found timing: {0} [Singleton]", methodRunner.Method);
			}
			else
			{
				Console.WriteLine(" Info:   Found timing: {0}", methodRunner.Method);
			}
		}

		/// <summary>
		/// Reports that a method is beginning its timing run.
		/// The same argument object is used for the start and end, but the
		/// Time will not be populated until the EndTimingMethod call.
		/// </summary>
		/// <param name="args">The shared arguments for timing information.</param>
		public void StartTimingMethod(TimingResultsArgs args)
		{}

		/// <summary>
		/// Reports the results of a single timing run.
		/// </summary>
		/// <param name="args">The shared arguments for timing information.</param>
		public void EndTimingMethod(TimingResultsArgs args)
		{
			Console.WriteLine(" Info: {2} = {0} at {1} iterations", args.Method, args.Iterations, args.Time);
		}

		#endregion Timing
	}
}
