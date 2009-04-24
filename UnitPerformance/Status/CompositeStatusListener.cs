using System.Collections.Generic;

namespace UnitPerformance.Status
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
	}
}
