using System.Collections.Generic;
using System.Linq;

namespace GuidUpdater.Matching;

public class DefaultMatcher : AssetMatcher
{
	public override bool Applies(AssetFile file1, AssetFile file2) => true;
	public override IEnumerable<KeyValuePair<long, long>> GetMatches(AssetFile file1, AssetFile file2) => Enumerable.Empty<KeyValuePair<long, long>>();
}
