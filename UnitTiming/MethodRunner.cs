using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnitPerformance
{
	/// <summary>
	/// Implements a runner that handles a method and the optional
	/// parameters for that method.
	/// </summary>
	public class MethodRunner
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodRunner"/> class.
		/// </summary>
		/// <param name="assemblyRunner">The assembly runner.</param>
		/// <param name="method">The method.</param>
		public MethodRunner(AssemblyRunner assemblyRunner, MethodInfo method)
			: this(assemblyRunner, method, method.GetParameters().Length)
		{}

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodRunner"/> class.
		/// </summary>
		/// <param name="assemblyRunner">The assembly runner.</param>
		/// <param name="method">The method.</param>
		/// <param name="methodSignature">The method signature.</param>
		public MethodRunner(AssemblyRunner assemblyRunner, MethodInfo method, int methodSignature)
		{
			// Save the parameters into fields.
			this.assemblyRunner = assemblyRunner;
			this.method = method;
			this.methodSignature = methodSignature;
		}

		#endregion Constructors

		#region Running

		/// <summary>
		/// Runs the specified iterator and maximum.
		/// </summary>
		/// <param name="fixture">The fixture.</param>
		/// <param name="iterator">The iterator.</param>
		/// <param name="maximum">The maximum.</param>
		public void Run(object fixture, int iterator, int maximum)
		{
			switch (Signature)
			{
				case 0:
					method.Invoke(fixture, null);
					break;
				case 1:
					method.Invoke(fixture, new object[] { iterator });
					break;
				case 2:
					method.Invoke(fixture, new object[] { iterator, maximum });
					break;
			}
		}

		#endregion

		#region Fields

		private AssemblyRunner assemblyRunner;
		private readonly MethodInfo method;
		private readonly int methodSignature;
		private PerformanceAttribute performanceAttribute = new PerformanceAttribute(1);

		/// <summary>
		/// Gets or sets the performance attribute.
		/// </summary>
		/// <value>The performance attribute.</value>
		public PerformanceAttribute PerformanceAttribute
		{
			get { return performanceAttribute; }
			set { performanceAttribute = value ?? new PerformanceAttribute(1); }
		}

		/// <summary>
		/// Gets the signature.
		/// </summary>
		/// <value>The signature.</value>
		public int Signature
		{
			get { return methodSignature; }
		}

		#endregion
	}
}
