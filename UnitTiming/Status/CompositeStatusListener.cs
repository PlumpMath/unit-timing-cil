using System;
using System.Collections.Generic;

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

		public void Error(string message, params object[] args)
		{
			foreach (IStatusListener listener in listeners)
			{
				listener.Error(message, args);
			}
		}

		#endregion

		#region Type Loading

		public void StartLoadingType(Type type)
		{
			foreach (IStatusListener listener in listeners)
			{
				listener.StartLoadingType(type);
			}
		}

		public void EndLoadingType(Type type)
		{
			foreach (IStatusListener listener in listeners)
			{
				listener.EndLoadingType(type);
			}
		}

		#endregion
	}
}
