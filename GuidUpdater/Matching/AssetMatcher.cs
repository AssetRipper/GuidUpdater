using System.Collections.Generic;

namespace GuidUpdater.Matching;
public abstract class AssetMatcher
{
	public abstract bool Applies(AssetFile file1, AssetFile file2);
	public abstract IEnumerable<KeyValuePair<long, long>> GetMatches(AssetFile file1, AssetFile file2);
}
