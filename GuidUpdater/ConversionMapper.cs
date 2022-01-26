using System;
using System.Collections.Generic;

namespace GuidUpdater
{
	internal static class ConversionMapper
    {
        internal static Dictionary<string, string> MakeConversionMap(string oldAssetsPath, string newAssetsPath)
        {
            //Relative path : guid string
            Dictionary<string, string> oldGuidMap = GuidParser.MakeGuidMap(oldAssetsPath);
            Dictionary<string, string> newGuidMap = GuidParser.MakeGuidMap(newAssetsPath);

            //old guid : new guid
            Dictionary<string, string> conversionMap = new Dictionary<string, string>();

            foreach((string oldPath, string oldGuid) in oldGuidMap)
            {
                if(newGuidMap.TryGetValue(oldPath, out string? newGuid))
                {
                    if(newGuid == null)
                        throw new Exception("Guid cannot be null");
                    conversionMap.Add(oldGuid, newGuid);
                }
            }

            return conversionMap;
        }
    }
}
