using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GuidUpdater;

public class Program
{
    public static void Main(string[] args)
    {
        //TestClass.Whatever();
        //GuidParser.SaveGuidMap(args[0], "guids.json");
        //DoReplacement(@"E:\TLD_Rip_Updating\TLD_Ripped_Combined\Assets", @"E:\TLD_Rip_Updating\146_Ripped\Assets");
        DoReplacement(args[0], args[1]);
        Console.WriteLine("Done!");
    }

    private static void DoReplacement(string oldAssetsDirectory, string newAssetsDirectory)
    {
        Stopwatch sw = Stopwatch.StartNew();
        Console.WriteLine($"Old Assets Directory: {oldAssetsDirectory}");
        Console.WriteLine($"New Assets Directory: {newAssetsDirectory}");
        Dictionary<string, string> conversionMap = ConversionMapper.MakeConversionMap(oldAssetsDirectory, newAssetsDirectory);
        sw.Stop();
        Console.WriteLine($"Conversion map completed in {sw.Elapsed.TotalSeconds} seconds.");
        sw.Restart();
        GuidReplacer.ReplaceGuids(oldAssetsDirectory, conversionMap);
        sw.Stop();
        Console.WriteLine($"Finished replacing guid's in {sw.Elapsed.TotalSeconds} seconds.");
    }
}