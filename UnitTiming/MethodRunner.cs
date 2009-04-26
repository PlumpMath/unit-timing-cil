using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UnitTiming
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
		/// <param name="target">The target.</param>
		/// <param name="methodSignature">The method signature.</param>
		public MethodRunner(TypeRunner assemblyRunner, MethodInfo method, object target, MethodSignature methodSignature)
		{
			// Check for null arguments.
			if (assemblyRunner == null)
			{
				throw new ArgumentNullException("assemblyRunner");
			}

			if (method == null)
			{
				throw new ArgumentNullException("method");
			}

			if (target == null)
			{
				throw new ArgumentNullException("target");
			}

			// Save the parameters into fields.
			this.assemblyRunner = assemblyRunner;
			this.method = method;
			this.target = target;
			this.methodSignature = methodSignature;
		}

		#endregion Constructors

		#region Running

		/// <summary>
		/// Runs the specified iterator and maximum.
		/// </summary>
		/// <param name="iterator">The iterator.</param>
		/// <param name="maximum">The maximum.</param>
		public void Run(int iterator, int maximum)
		{
			switch (MethodSignature)
			{
			case MethodSignature.Zero:
				method.Invoke(target, new object[] {});
				break;
			case MethodSignature.CountInt32:
				method.Invoke(target, new object[] { iterator });
				break;
			case MethodSignature.CountIterationInt32:
				method.Invoke(target, new object[] { iterator, maximum });
				break;
			}
		}

		#endregion

		#region Fields

		private TypeRunner assemblyRunner;
		private readonly MethodInfo method;
		private readonly MethodSignature methodSignature;
		private readonly object target;
		private TimingAttribute performanceAttribute = new TimingAttribute(1);

		/// <summary>
		/// Gets or sets the performance attribute.
		/// </summary>
		/// <value>The performance attribute.</value>
		public TimingAttribute TimingAttribute
		{
			get { return performanceAttribute; }
			set { performanceAttribute = value ?? new TimingAttribute(1); }
		}

		/// <summary>
		/// Gets the signature.
		/// </summary>
		/// <value>The signature.</value>
		public MethodSignature MethodSignature
		{
			get { return methodSignature; }
		}

		#endregion
	}
}