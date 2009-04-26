using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnitTiming.Status
{
	/// <summary>
	/// Implements a composite listener that sends the various events
	/// to the contained listeners.
	/// </summary>
	public class CompositeStatusListener
		: IStatusListener
	{
		#region Listeners

		private readonly List<IStatusListener> listeners = new List<IStatusListener>();

		public List<IStatusListener> Listeners
		{
			get { return listeners; }
		}

		#endregion Listeners

		#region Status Events

		#endregion Status Events

		#region General Messages

		/// <summary>
		/// Reports a specific error message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="args">The args.</param>
		public void Error(string message, params object[] args)
		{
			listeners.ForEach(sl => sl.Error(message, args));
		}

		#endregion

		#region Type Loading

		/// <summary>
		/// Reports that the system is beginning to scan through a type looking
		/// for tests.
		/// </summary>
		/// <param name="type">The type which is being scanned.</param>
		public void StartLoadingType(Type type)
		{
			listeners.ForEach(sl => sl.StartLoadingType(type));
		}

		/// <summary>
		/// Reports that a given type is finished being scanned for tests.
		/// </summary>
		/// <param name="type">The type being scanned.</param>
		public void EndLoadingType(Type type)
		{
			listeners.ForEach(sl => sl.EndLoadingType(type));
		}

		#endregion

		#region Timing

		/// <summary>
		/// Reports that a timing method was found.
		/// </summary>
		/// <param name="methodRunner">The method which is being added.</param>
		public void AddTimingMethod(MethodRunner methodRunner)
		{
			listeners.ForEach(sl => sl.AddTimingMethod(methodRunner));
		}

		/// <summary>
		/// Reports that a method is beginning its timing run.
		/// The same argument object is used for the start and end, but the
		/// Time will not be populated until the EndTimingMethod call.
		/// </summary>
		/// <param name="args">The shared arguments for timing information.</param>
		public void StartTimingMethod(TimingResultsArgs args)
		{
			listeners.ForEach(sl => sl.StartTimingMethod(args));
		}

		/// <summary>
		/// Reports the results of a single timing run.
		/// </summary>
		/// <param name="args">The shared arguments for timing information.</param>
		public void EndTimingMethod(TimingResultsArgs args)
		{
			listeners.ForEach(sl => sl.EndTimingMethod(args));
		}

		#endregion Timing
	}
}
