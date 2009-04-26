using System;

using MfGames.Logging;

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
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ConsoleStatusListener"/> class.
		/// </summary>
		public ConsoleStatusListener()
		{
			this.log = new Log(GetType());
		}

		#endregion

		#region Status Events
		#endregion Status Events

		#region Logging

		private Log log;

		#endregion

		#region General Messages

		public void Error(string message, params object[] args)
		{
			log.Error(message, args);
		}

		#endregion

		#region Type Loading

		public void StartLoadingType(Type type)
		{
			log.Info("Loading type: {0}", type);
		}

		public void EndLoadingType(Type type)
		{
		}

		#endregion
	}
}
