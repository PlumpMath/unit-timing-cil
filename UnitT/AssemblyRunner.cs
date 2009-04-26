using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using UnitPerformance.Collections;

namespace UnitPerformance
{
	/// <summary>
	/// Encapsulates the functionality for running performance tests from a given type.
	/// </summary>
	public class AssemblyRunner
	{
		#region Constants

		private static readonly Type[] ZeroParameter = new Type[] { };
		private static readonly Type[] OneParameter = new Type[] { typeof(int) };
		private static readonly Type[] TwoParameter = new Type[] { typeof(int), typeof(int) };

		#endregion Constants

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyRunner"/> class.
		/// </summary>
		/// <param name="runner">The runner.</param>
		/// <param name="type">The type.</param>
		public AssemblyRunner(Runner runner, Type type)
		{
			// Save the values in the container.
			this.runner = runner;
			this.type = type;

			// Make sure we can construct the object.
			constructor = type.GetConstructor(new Type[] { });

			if (constructor == null)
			{
				throw new Exception("Cannot find default constructor for " + type);
			}

			// Go through all the methods within the type.
			foreach (MethodInfo method in type.GetMethods())
			{
				// Figure out the signature of this method.
				int methodSignature = GetParameterSignature(method);

				if (methodSignature == -1)
				{
					continue;
				}

				// Get the attributes for the method.
				var methodRunner = new MethodRunner(this, method, methodSignature);

				foreach (Attribute attribute in method.GetCustomAttributes(false))
				{
					// Check for [PerformanceFixtureSetup]
					if (attribute is PerformanceFixtureSetupAttribute)
					{
						if (methodSignature == 0)
						{
							fixtureSetupMethods.Add(method);
						}
					}

					// Check for [PerformanceFixtureTeardown]
					if (attribute is PerformanceFixtureTeardownAttribute)
					{
						if (methodSignature == 0)
						{
							fixtureTeardownMethods.Add(method);
						}
					}

					// Check for [PerformanceSetup]
					if (attribute is PerformanceSetupAttribute)
					{
						setupMethods.Add(methodRunner);
					}

					// Check for [PerformanceTeardown]
					if (attribute is PerformanceTeardownAttribute)
					{
						teardownMethods.Add(methodRunner);
					}

					// Check for [PerformanceBaseline]
					if (attribute is PerformanceBaselineAttribute)
					{
						baselineMethods[methodSignature] = methodRunner;
					}

					// Check for [Performance]
					if (attribute is PerformanceAttribute)
					{
						methodRunner.PerformanceAttribute = (PerformanceAttribute) attribute;
						methods.Add(methodRunner);
					}
				}
			}

			// Set up the baselines if we don't have one.
			if (baselineMethods[0] == null)
				baselineMethods[0] = new MethodRunner(this, GetType().GetMethod("Baseline", new Type[]{}));

			if (baselineMethods[1] == null)
				baselineMethods[1] = new MethodRunner(this, GetType().GetMethod("Baseline", new[] { typeof(int) }));

			if (baselineMethods[2] == null)
				baselineMethods[2] = new MethodRunner(this, GetType().GetMethod("Baseline", new[] { typeof(int), typeof(int) }));
		}

		#endregion

		#region Baseline Methods

		/// <summary>
		/// Implements the basic baseline method for testing.
		/// </summary>
		public void Baseline()
		{}

		/// <summary>
		/// Implements the basic baseline method for testing.
		/// </summary>
		/// <param name="count">The count.</param>
		public void Baseline(int count)
		{}

		/// <summary>
		/// Implements the basic baseline method for testing.
		/// </summary>
		/// <param name="count">The count.</param>
		/// <param name="maximum">The maximum.</param>
		public void Baseline(int count, int maximum)
		{}

		#endregion

		#region Query Methods

		/// <summary>
		/// Gets the parameter signature. This returns -1 for an invalid
		/// signature or a number between 0-2 depending on the parameters.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		private static int GetParameterSignature(MethodInfo method)
		{
			// Get the parameters and check them out.
			ParameterInfo[] parameters = method.GetParameters();

			// Zero parameters are the easiest.
			if (parameters.Length == 0)
			{
				return 0;
			}

			// If there are more than 2 parameters, we don't bother.
			if (parameters.Length > 2)
			{
				return -1;
			}

			// Check the parameters. We only accept integers at this point.
			foreach (ParameterInfo parameter in parameters)
			{
				// Make sure we are accepting a valid type.
				if (parameter.ParameterType != typeof(int))
				{
					// We only accept integer types.
					return -1;
				}

				// Check for invalid modifiers to the parameter.
				if (parameter.IsOut ||
				    parameter.IsRetval)
				{
					// We can't handle "out" and "ref" parameters.
					return -1;
				}
			}

			// If we got this far, than we have a valid parameter. We return
			// the number of integers we allow.
			return parameters.Length;
		}

		#endregion

		#region Running

		/// <summary>
		/// Runs the various assembly tests.
		/// </summary>
		public void Run()
		{
			// Create a new object of the fixture.
			object fixture = constructor.Invoke(new object[]{});

			// Start the fixture testing.
			RunFixtureSetupMethods(fixture);

			// Run the unit performance blocks.
			RunPerformanceMethods(fixture);

			// Finish out the fixture setup.
			RunFixtureTeardownMethods(fixture);
		}

		/// <summary>
		/// Runs the fixture setup methods.
		/// </summary>
		/// <param name="fixture">The fixture.</param>
		private void RunFixtureSetupMethods(object fixture)
		{
			foreach (MethodInfo method in fixtureSetupMethods)
			{
				method.Invoke(fixture, new object[] { });
			}
		}

		/// <summary>
		/// Runs the fixture teardown methods.
		/// </summary>
		/// <param name="fixture">The fixture.</param>
		private void RunFixtureTeardownMethods(object fixture)
		{
			foreach (MethodInfo method in fixtureTeardownMethods)
			{
				method.Invoke(fixture, new object[] { });
			}
		}

		/// <summary>
		/// Runs the unit performance in the fixture.
		/// </summary>
		/// <param name="fixture">The fixture.</param>
		private void RunPerformanceMethods(object fixture)
		{
			// Go through the performance tests.
			foreach (MethodRunner methodRunner in methods)
			{
				// Keep track of the execution time, in ticks.
				var executionTime = new TimeSpan();

				// Go through the iterations for the performance.
				foreach (int iteration in methodRunner.PerformanceAttribute.Iterations)
				{
					// Get the time to run the baseline and also the time to run the performance unit.
					TimeSpan baselineTime = GetBaselineExecutionTime(fixture, iteration, methodRunner.Signature);
					TimeSpan methodTime = GetExecutionTime(methodRunner, iteration, fixture);
					executionTime += (methodTime - baselineTime);

					// Report the results of the timing.
					Console.WriteLine("Time to run: {0}", executionTime);
				}
			}
		}

		#endregion

		#region Running - Baseline

		/// <summary>
		/// Gets the baseline execution time for a given signature and iteration.
		/// </summary>
		/// <param name="fixture">The fixture.</param>
		/// <param name="iteration">The iteration.</param>
		/// <param name="methodSignature">The method signature.</param>
		/// <returns></returns>
		private TimeSpan GetBaselineExecutionTime(object fixture, int iteration, int methodSignature)
		{
			return GetExecutionTime(baselineMethods[methodSignature], iteration, fixture);
		}

		/// <summary>
		/// Gets the execution time.
		/// </summary>
		/// <param name="methodRunner">The method runner.</param>
		/// <param name="iteration">The iteration.</param>
		/// <param name="fixture">The fixture.</param>
		/// <returns></returns>
		private static TimeSpan GetExecutionTime(MethodRunner methodRunner, int iteration, object fixture)
		{
			// Run the setup functions.

			// Loop through the iterations. It is very important to minimum
			// the processing in this stanza.
			DateTime startTime = DateTime.UtcNow;

			for (int run = 0; run < iteration; run++)
			{
				methodRunner.Run(fixture, run, iteration);
			}

			// Run the teardown methods.

			// Return the results of the execution.
			return DateTime.UtcNow - startTime;
		}

		#endregion

		#region Fields

		private readonly Runner runner;
		private readonly Type type;
		private readonly ConstructorInfo constructor;

		private readonly List<MethodInfo> fixtureSetupMethods = new List<MethodInfo>();
		private readonly List<MethodInfo> fixtureTeardownMethods = new List<MethodInfo>();
		private readonly MethodRunnerList setupMethods = new MethodRunnerList();
		private readonly MethodRunnerList teardownMethods = new MethodRunnerList();
		private readonly MethodRunnerList methods = new MethodRunnerList();

		private readonly MethodRunner[] baselineMethods = new MethodRunner[]
		                                         { null, null, null };

		#endregion
	}
}