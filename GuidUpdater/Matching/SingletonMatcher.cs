using System.Collections.Generic;

namespace GuidUpdater.Matching;

public class SingletonMatcher : AssetMatcher
{
	public override bool Applies(AssetFile file1, AssetFile file2)
	{
		return file1.Count == 1 && file2.Count == 1 && file1[0].ClassID == file2[0].ClassID;
	}

	public override IEnumerable<KeyValuePair<long, long>> GetMatches(AssetFile file1, AssetFile file2)
	{
		yield return new KeyValuePair<long, long>(file1[0].FileID, file2[0].FileID);
	}
}