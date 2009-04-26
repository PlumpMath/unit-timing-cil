using System;
using System.Collections.Generic;
using System.Reflection;

using UnitTiming.Collections;
using UnitTiming.Status;

namespace UnitTiming
{
	/// <summary>
	/// Encapsulates the functionality for running performance tests from a given type.
	/// </summary>
	public class TypeRunner
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeRunner"/> class.
		/// </summary>
		/// <param name="runner">The runner.</param>
		/// <param name="type">The type.</param>
		public TypeRunner(Runner runner, Type type)
		{
			// Make sure the parameters are not null.
			if (runner == null)
			{
				throw new ArgumentNullException("runner");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			// Save the values in the container.
			this.runner = runner;
			this.type = type;

			// Report the assembly is loading.
			runner.StatusListener.StartLoadingType(type);

			try
			{
				// Create the fixture object.
				CreateFixture();

				if (fixture == null)
				{
					return;
				}

				// Go through all the methods within the type.
				foreach (MethodInfo method in type.GetMethods())
				{
					// Figure out the signature of this method.
					ParseMethod(method);
				}

				// Set up the baselines if we don't have one.
				AssignDefaultBaselines();
			}
			finally
			{
				// Report the end of the type loading.
				runner.StatusListener.EndLoadingType(type);
			}
		}

		#endregion

		#region Baseline Methods

		public void LookAtMe(object target, int iterator)
		{
			for (int count = 0; count < iterator; count++)
			{
				Baseline();
			}
		}

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

		#region Reflection Methods

		/// <summary>
		/// Assigns the default baselines if one hasn't already been assigned.
		/// </summary>
		private void AssignDefaultBaselines()
		{
			if (baselineMethods[0] == null)
			{
				MethodInfo zeroMethod = GetType().GetMethod("Baseline",
				                                            new Type[] { });
				baselineMethods[0] = new MethodRunner(this,
				                                      zeroMethod,
				                                      this,
				                                      MethodSignature.Zero);
			}

			if (baselineMethods[1] == null)
			{
				MethodInfo countMethod = GetType().GetMethod("Baseline",
				                                             new[]
				                                             { typeof(int) });
				baselineMethods[1] = new MethodRunner(this,
				                                      countMethod,
				                                      this,
				                                      MethodSignature.CountInt32);
			}

			if (baselineMethods[2] == null)
			{
				MethodInfo countIterationMethod = GetType().GetMethod("Baseline",
				                                                      new[]
				                                                      {
				                                                      	typeof(int),
				                                                      	typeof(int)
				                                                      });
				baselineMethods[2] = new MethodRunner(this,
				                                      countIterationMethod,
				                                      this,
				                                      MethodSignature.CountIterationInt32);
			}
		}

		/// <summary>
		/// Creates the fixture object.
		/// </summary>
		private void CreateFixture()
		{
			// Create the object in question so we can target tasks.
			ConstructorInfo constructor = type.GetConstructor(new Type[] { });

			if (constructor != null)
			{
				fixture = constructor.Invoke(new object[] { });
			}
			else
			{
				runner.StatusListener.Error("Type does not have empty constructor");
			}
		}

		/// <summary>
		/// Gets the parameter signature. This returns -1 for an invalid
		/// signature or a number between 0-2 depending on the parameters.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		private static MethodSignature GetMethodSignature(MethodInfo method)
		{
			// Get the parameters and check them out.
			ParameterInfo[] parameters = method.GetParameters();

			// Zero parameters are the easiest.
			if (parameters.Length == 0)
			{
				return MethodSignature.Zero;
			}

			// Check the parameters. We only accept integers at this point.
			foreach (ParameterInfo parameter in parameters)
			{
				// Make sure we are accepting a valid type.
				if (parameter.ParameterType != typeof(int))
				{
					// We only accept integer types.
					return MethodSignature.Unknown;
				}

				// Check for invalid modifiers to the parameter.
				if (parameter.IsOut ||
				    parameter.IsRetval)
				{
					// We can't handle "out" and "ref" parameters.
					return MethodSignature.Unknown;
				}
			}

			// If we got this far, than we have a valid parameter. We return
			// the number of integers we allow.
			switch (parameters.Length)
			{
			case 1:
				return MethodSignature.CountInt32;
			case 2:
				return MethodSignature.CountIterationInt32;
			default:
				return MethodSignature.Unknown;
			}
		}

		/// <summary>
		/// Parses the method info object and adds the appropriate tests.
		/// </summary>
		/// <param name="method">The method.</param>
		private void ParseMethod(MethodInfo method)
		{
			// Make sure we have a valid signature.
			MethodSignature methodSignature = GetMethodSignature(method);

			if (methodSignature == MethodSignature.Unknown)
			{
				return;
			}

			// Get the attributes for the method.
			var methodRunner = new MethodRunner(this, method, fixture, methodSignature);

			foreach (Attribute attribute in method.GetCustomAttributes(false))
			{
				// Check for [TimingFixtureSetup].
				ParseTimingFixtureSetup(method, methodSignature, attribute);

				// Check for [TimingFixtureTeardown].
				ParseTimingFixtureTeardown(method, methodSignature, attribute);

				// Check for [TimingSetup].
				ParseTimingSetup(method, methodSignature, attribute, methodRunner);

				// Check for [TimingTeardown].
				ParseTimingTeardown(method, methodSignature, attribute, methodRunner);

				// Check for [TimingBaseline].
				ParseTimingBaseline(methodSignature, attribute, methodRunner);

				// Check for [Timing]
				ParseTiming(attribute, methodRunner);
			}
		}

		private void ParseTiming(Attribute attribute, MethodRunner methodRunner)
		{
			if (!(attribute is TimingAttribute))
			{
				return;
			}

			// Add this as an actual timing unit.
			methodRunner.TimingAttribute = (TimingAttribute) attribute;
			methods.Add(methodRunner);

			// Report that this method has been added.
			runner.StatusListener.AddTimingMethod(methodRunner);
		}

		private void ParseTimingBaseline(
			MethodSignature methodSignature,
			Attribute attribute,
			MethodRunner methodRunner)
		{
			if (!(attribute is TimingBaselineAttribute))
			{
				return;
			}

			switch (methodSignature)
			{
			case MethodSignature.Zero:
				baselineMethods[0] = methodRunner;
				break;
			case MethodSignature.CountInt32:
				baselineMethods[1] = methodRunner;
				break;
			case MethodSignature.CountIterationInt32:
				baselineMethods[2] = methodRunner;
				break;
			}
		}

		private void ParseTimingSetup(
			MethodInfo method,
			MethodSignature methodSignature,
			Attribute attribute,
			MethodRunner methodRunner)
		{
			if (!(attribute is TimingSetupAttribute))
			{
				return;
			}

			switch (methodSignature)
			{
			case MethodSignature.Zero:
			case MethodSignature.CountInt32:
				setupMethods.Add(methodRunner);
				break;
			default:
				runner.StatusListener.Error(
					"{0}: [TimingSetup] takes zero or one integer.", method.Name);
				break;
			}
		}

		private void ParseTimingTeardown(
			MethodInfo method,
			MethodSignature methodSignature,
			Attribute attribute,
			MethodRunner methodRunner)
		{
			if (!(attribute is TimingTeardownAttribute))
			{
				return;
			}

			switch (methodSignature)
			{
			case MethodSignature.Zero:
			case MethodSignature.CountInt32:
				teardownMethods.Add(methodRunner);
				break;
			default:
				runner.StatusListener.Error(
					"{0}: [TimingTeardown] takes zero or one integer.", method.Name);
				break;
			}
		}

		private void ParseTimingFixtureTeardown(
			MethodInfo method, MethodSignature methodSignature, Attribute attribute)
		{
			if (!(attribute is TimingFixtureTeardownAttribute))
			{
				return;
			}

			if (methodSignature == MethodSignature.Zero)
			{
				fixtureTeardownMethods.Add(method);
			}
			else
			{
				runner.StatusListener.Error(
					"{0}: [TimingFixtureTeardown] cannot take parameters.", method.Name);
			}
		}

		private void ParseTimingFixtureSetup(
			MethodInfo method, MethodSignature methodSignature, Attribute attribute)
		{
			if (!(attribute is TimingFixtureSetupAttribute))
			{
				return;
			}

			if (methodSignature == MethodSignature.Zero)
			{
				fixtureSetupMethods.Add(method);
			}
			else
			{
				runner.StatusListener.Error(
					"{0}: [TimingFixtureSetup] cannot take parameters.", method.Name);
			}
		}

		#endregion

		#region Running

		/// <summary>
		/// Gets the baseline execution time for a given signature and iteration.
		/// </summary>
		/// <param name="iteration">The iteration.</param>
		/// <param name="methodSignature">The method signature.</param>
		/// <returns></returns>
		private TimeSpan GetBaselineExecutionTime(MethodSignature methodSignature, int iteration)
		{
			return baselineMethods[(int) methodSignature].GetExecutionTime(iteration);
		}

		/// <summary>
		/// Runs the various assembly tests.
		/// </summary>
		public void Run()
		{
			// Start the fixture testing.
			RunFixtureSetupMethods();

			// Run the unit performance blocks.
			RunTimingMethods();

			// Finish out the fixture setup.
			RunFixtureTeardownMethods();
		}

		/// <summary>
		/// Runs the fixture setup methods.
		/// </summary>
		private void RunFixtureSetupMethods()
		{
			foreach (MethodInfo method in fixtureSetupMethods)
			{
				method.Invoke(fixture, new object[] { });
			}
		}

		/// <summary>
		/// Runs the fixture teardown methods.
		/// </summary>
		private void RunFixtureTeardownMethods()
		{
			foreach (MethodInfo method in fixtureTeardownMethods)
			{
				method.Invoke(fixture, new object[] { });
			}
		}

		/// <summary>
		/// Runs the unit performance in the fixture.
		/// </summary>
		private void RunTimingMethods()
		{
			// Go through the performance tests.
			foreach (MethodRunner methodRunner in methods)
			{
				// Go through the iterations for the performance.
				foreach (int iteration in methodRunner.TimingAttribute.Iterations)
				{
					// Get the time to run the baseline. The normal baseline will probably be
					// zero, but the user may give a longer-running one.
					TimeSpan baselineTime = GetBaselineExecutionTime(methodRunner.MethodSignature, iteration);

					// Report that we are starting to run the timing.
					var timingResults = new TimingResultsArgs
					{
						Iterations = iteration,
						Method = methodRunner.Method,
						BaselineTime = baselineTime,
					};
					Runner.StatusListener.StartTimingMethod(timingResults);

					TimeSpan methodTime = methodRunner.GetExecutionTime(iteration);

					// Report the results of the timing.
					timingResults.Time = (methodTime - baselineTime);
					runner.StatusListener.EndTimingMethod(timingResults);
				}
			}
		}

		#endregion

		#region Fields

		private readonly MethodRunner[] baselineMethods = new MethodRunner[3];

		private readonly List<MethodInfo> fixtureSetupMethods = new List<MethodInfo>();

		private readonly List<MethodInfo> fixtureTeardownMethods =
			new List<MethodInfo>();

		private readonly MethodRunnerList methods = new MethodRunnerList();
		private readonly Runner runner;

		private readonly MethodRunnerList setupMethods = new MethodRunnerList();
		private readonly MethodRunnerList teardownMethods = new MethodRunnerList();
		private readonly Type type;
		private object fixture;

		/// <summary>
		/// Gets the top-most runner object.
		/// </summary>
		/// <value>The runner.</value>
		public Runner Runner
		{
			get { return runner; }
		}

		#endregion
	}
}