using System;
using System.Reflection;

namespace UnitTiming.Status
{
	/// <summary>
	/// Represents a listener object which is attached to a runner and
	/// listens to events from the run.
	/// </summary>
	public interface IStatusListener
	{
		#region General Messages

		void Error(string message, params object[] args);

		#endregion

		#region Type Loading

		void StartLoadingType(Type type);

		void EndLoadingType(Type type);

		#endregion

		#region Timing

		/// <summary>
		/// Reports that a timing method was found.
		/// </summary>
		/// <param name="methodRunner">The method which is being added.</param>
		void AddTimingMethod(MethodRunner methodRunner);

		/// <summary>
		/// Reports that a method is beginning its timing run.
		/// 
		/// The same argument object is used for the start and end, but the
		/// Time will not be populated until the EndTimingMethod call.
		/// </summary>
		/// <param name="args">The shared arguments for timing information.</param>
		void StartTimingMethod(TimingResultsArgs args);

		/// <summary>
		/// Reports the results of a single timing run.
		/// </summary>
		/// <param name="args">The shared arguments for timing information.</param>
		void EndTimingMethod(TimingResultsArgs args);

		#endregion
	}
}