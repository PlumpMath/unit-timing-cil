using System;

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
	}
}