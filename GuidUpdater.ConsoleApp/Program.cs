using System.Diagnostics;

namespace GuidUpdater.ConsoleApp;

public class Program
{
    public static void Main(string[] args)
    {
        //DoReplacement(@"FirstProject\Assets", @"SecondProject\Assets");
        DoReplacement(args[0], args[1]);
        Console.WriteLine("Done!");
    }

    private static void DoReplacement(string oldAssetsDirectory, string newAssetsDirectory)
    {
        Stopwatch sw = Stopwatch.StartNew();
        Console.WriteLine($"Old Assets Directory: {oldAssetsDirectory}");
        Console.WriteLine($"New Assets Directory: {newAssetsDirectory}");
		PathMapper.Map(oldAssetsDirectory, newAssetsDirectory);
		GuidMapper.Map();
		Scripts.ScriptMapper.BuildMapping();
        sw.Stop();
        Console.WriteLine($"Guid mapping completed in {sw.Elapsed.TotalSeconds} seconds.");
        sw.Restart();
		PPtrMapper.Map(oldAssetsDirectory, newAssetsDirectory);
		sw.Stop();
		Console.WriteLine($"PPtr mapping completed in {sw.Elapsed.TotalSeconds} seconds.");
		sw.Restart();
        ReferenceUpdater.UpdateReferencesInDirectory(oldAssetsDirectory);
        sw.Stop();
        Console.WriteLine($"Finished updating references in {sw.Elapsed.TotalSeconds} seconds.");
    }
}