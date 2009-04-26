using System;
using System.Reflection;
using System.Reflection.Emit;

namespace UnitTiming
{
	/// <summary>
	/// Defines the common delegate for performing dynamic unit testing.
	/// </summary>
	internal delegate void DynamicUnitTiming(object target, int iterations);

	/// <summary>
	/// Creates a custom testing code.
	/// </summary>
	internal static class MethodRunnerCompiler
	{
		#region Compiler

		/// <summary>
		/// Creates a dynamic method for testing units. All units take the same parameters.
		/// </summary>
		/// <param name="runner">The runner.</param>
		/// <returns></returns>
		public static DynamicUnitTiming CreateTestMethod(MethodRunner runner)
		{
			// Create the basic signature.
			var signature = new[] { typeof(object), typeof(int) };
			var method = new DynamicMethod("DynamicUnitTest" + (counter++),
			                               typeof(void),
			                               signature,
			                               typeof(MethodRunnerCompiler));

			// Create an IL generator for the method body.
			ILGenerator il = method.GetILGenerator(256);

			// We determine what type of IL code we generate based on the singleton type.
			if (runner.TimingAttribute.Singleton)
			{
				// Create a single call version.
				CreateSingleton(runner, il);
			}
			else
			{
				// Create the local variables.
				il.DeclareLocal(typeof(int));
				il.DeclareLocal(typeof(bool));

				// Declare the labels.
				Label loopLabel = il.DefineLabel();
				Label topLabel = il.DefineLabel();

				// Assign zero to the count variable.
				il.Emit(OpCodes.Ldc_I4_0);
				il.Emit(OpCodes.Stloc_0);
				il.Emit(OpCodes.Br_S, loopLabel);

				// Build up the actual execution.
				il.MarkLabel(topLabel);

				// Figure out how to call this method.
				il.Emit(OpCodes.Ldarg_0);

				switch (runner.MethodSignature)
				{
				case MethodSignature.CountInt32:
					il.Emit(OpCodes.Ldloc_0);
					break;
				case MethodSignature.CountIterationInt32:
					il.Emit(OpCodes.Ldloc_0);
					il.Emit(OpCodes.Ldarg_1);
					break;
				}

				il.EmitCall(OpCodes.Call, runner.Method, null);

				// Increment the counter.
				il.Emit(OpCodes.Ldloc_0);
				il.Emit(OpCodes.Ldc_I4_1);
				il.Emit(OpCodes.Add);
				il.Emit(OpCodes.Stloc_0);

				// Create the loop test. This loads the count variable and compares
				// it to the second argument (iterations).
				il.MarkLabel(loopLabel);

				il.Emit(OpCodes.Ldloc_0);
				il.Emit(OpCodes.Ldarg_1);
				il.Emit(OpCodes.Clt);

				il.Emit(OpCodes.Stloc_1);
				il.Emit(OpCodes.Ldloc_1);
				il.Emit(OpCodes.Brtrue_S, topLabel);
			}

			// Finish up with a return IL.
			il.Emit(OpCodes.Ret);

			// Create the paramters.
			method.DefineParameter(0, ParameterAttributes.In, "target");
			method.DefineParameter(1, ParameterAttributes.In, "iteration");

			// Create the delegate and return it.
			return (DynamicUnitTiming) method.CreateDelegate(typeof(DynamicUnitTiming));
		}

		#endregion

		#region Emitting Methods

		/// <summary>
		/// Creates the singleton dynamic test.
		/// </summary>
		/// <param name="runner">The runner.</param>
		/// <param name="il">The il.</param>
		private static void CreateSingleton(MethodRunner runner, ILGenerator il)
		{
			// Set up the call to the method.
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.EmitCall(OpCodes.Call, runner.Method, null);
		}

		#endregion

		#region Fields

		private static int counter;

		#endregion
	}
}
