namespace UnitTiming
{
	public enum MethodSignature
	{
		/// <summary>
		/// Unknown signature method.
		/// </summary>
		Unknown,

		/// <summary>
		/// No parameters in the method.
		/// </summary>
		Zero,

		/// <summary>
		/// The method has a single iteration, that takes an integer.
		/// </summary>
		CountInt32,

		/// <summary>
		/// The method takes an iteration counter and a maximum amount.
		/// </summary>
		CountIterationInt32,
	}
}
