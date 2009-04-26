using System;
using System.IO;

using UnitPerformance;
using UnitPerformance.Status;

namespace UnitPerformanceTool
{
	/// <summary>
	/// A simple tool for scanning DLL's and performing unit tests
	/// on them.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// Indicates the main entry into the application.
		/// </summary>
		/// <param name="args">The args.</param>
		static void Main(string[] args)
		{
			// Create the performance runner.
			var runner = new Runner();
			runner.StatusListener.Listeners.Add(new ConsoleStatusListener());

			// Go through the arguments and load each one through the
			// assembly loader or set up the command-line arguments.
			foreach (string arg in args)
			{
				// See if this is a DLL or EXE file.
				if (File.Exists(arg))
				{
					// Load the assembly into the runner.
					runner.LoadAssembly(arg);
				}
			}

			// Once we are done loading, perform the actual performance
			// testing of the various units.
			runner.Run();

			Console.Write("Done.");
			Console.ReadKey();
		}
	}
}
