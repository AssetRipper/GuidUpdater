using System.Diagnostics;

namespace GuidUpdater.ConsoleApp;

public class Program
{
	public static void Main(string[] args)
	{
		ProjectUpdater.UpdateProject(args[0], args[1]);
		Console.WriteLine("Done!");
	}
}