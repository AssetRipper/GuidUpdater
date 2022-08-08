using System.Diagnostics;

namespace GuidUpdater.ConsoleApp;

public class Program
{
    public static void Main(string[] args)
    {
        DoReplacement(@"FirstProject\Assets", @"SecondProject\Assets");
        //DoReplacement(args[0], args[1]);
        Console.WriteLine("Done!");
    }

    private static void DoReplacement(string oldAssetsDirectory, string newAssetsDirectory)
    {
        Stopwatch sw = Stopwatch.StartNew();
        Console.WriteLine($"Old Assets Directory: {oldAssetsDirectory}");
        Console.WriteLine($"New Assets Directory: {newAssetsDirectory}");
        GuidParser.MakeMapping(oldAssetsDirectory, newAssetsDirectory);
        sw.Stop();
        Console.WriteLine($"Mapping completed in {sw.Elapsed.TotalSeconds} seconds.");
        sw.Restart();
        GuidReplacer.UpdateReferencesInDirectory(oldAssetsDirectory);
        sw.Stop();
        Console.WriteLine($"Finished updating references in {sw.Elapsed.TotalSeconds} seconds.");
    }
}