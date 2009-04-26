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
		/// <param name="typeRunner">The assembly runner.</param>
		/// <param name="method">The method.</param>
		/// <param name="target">The target.</param>
		/// <param name="methodSignature">The method signature.</param>
		public MethodRunner(TypeRunner typeRunner, MethodInfo method, object target, MethodSignature methodSignature)
		{
			// Check for null arguments.
			if (typeRunner == null)
			{
				throw new ArgumentNullException("typeRunner");
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
			this.typeRunner = typeRunner;
			this.method = method;
			this.target = target;
			this.methodSignature = methodSignature;
		}

		#endregion Constructors

		#region Running


		/// <summary>
		/// Gets the execution time.
		/// </summary>
		/// <param name="iteration">The iteration.</param>
		/// <returns></returns>
		public TimeSpan GetExecutionTime(int iteration)
		{
			// Dynamically generation the method.
			if (dynamicMethod == null)
			{
				dynamicMethod = MethodRunnerCompiler.CreateTestMethod(this);
			}

			// Run the dynamic method. It is important that nothing else is done
			// between the type timestamps.
			DateTime startTime = DateTime.UtcNow;
			dynamicMethod(target, iteration);
			return DateTime.UtcNow - startTime;
		}

		#endregion

		#region Fields

		private readonly TypeRunner typeRunner;
		private readonly MethodInfo method;
		private readonly MethodSignature methodSignature;
		private readonly object target;
		private TimingAttribute performanceAttribute = new TimingAttribute(1);
		private DynamicUnitTiming dynamicMethod;

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
		/// Gets the method that will be called.
		/// </summary>
		/// <value>The method.</value>
		public MethodInfo Method
		{
			get { return method; }
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