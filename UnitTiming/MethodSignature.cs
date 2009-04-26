namespace UnitTiming
{
	public enum MethodSignature
	{
		/// <summary>
		/// Unknown signature method.
		/// </summary>
		Unknown = -1,

		/// <summary>
		/// No parameters in the method.
		/// </summary>
		Zero = 0,

		/// <summary>
		/// The method has a single iteration, that takes an integer.
		/// </summary>
		CountInt32 = 1,

		/// <summary>
		/// The method takes an iteration counter and a maximum amount.
		/// </summary>
		CountIterationInt32 = 2,
	}
}
