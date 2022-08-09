using GuidUpdater.Assets;
using System.Collections.Generic;
using System.Linq;
using NamedObjectDictionary = System.Collections.Generic.Dictionary<(int, string?, int), GuidUpdater.Assets.NamedObject>;

namespace GuidUpdater.Matching;

public class NamedObjectMatcher : AssetMatcher
{
	public override bool Applies(AssetFile file1, AssetFile file2)
	{
		return file1.Assets.Any(asset => NamedObject.IsNamedObject(asset.ClassID)) 
			&& file2.Assets.Any(asset => NamedObject.IsNamedObject(asset.ClassID));
	}
	
	public override IEnumerable<KeyValuePair<long, long>> GetMatches(AssetFile file1, AssetFile file2)
	{
		NamedObjectDictionary childDictionary1 = MakeNamedObjectDictionary(file1);
		NamedObjectDictionary childDictionary2 = MakeNamedObjectDictionary(file2);
		foreach (((int, string?, int) key, NamedObject object1) in childDictionary1)
		{
			if (childDictionary2.TryGetValue(key, out NamedObject object2))
			{
				yield return new KeyValuePair<long, long>(object1.Asset.FileID, object2.Asset.FileID);
			}
		}
	}

	private static NamedObjectDictionary MakeNamedObjectDictionary(AssetFile file)
	{
		Dictionary<long, UnityAsset> dictionary = file.MakeAssetDictionary();
		NamedObjectDictionary result = new();
		foreach (UnityAsset asset in file.Assets)
		{
			int id = asset.ClassID;
			if (NamedObject.IsNamedObject(id))
			{
				NamedObject namedObject = new NamedObject(asset, dictionary);
				string? name = namedObject.IsAnimatorTransition ? ((AnimatorTransition)namedObject).Identifier : namedObject.Name;
				int nameIndex = 0;//The number of objects with the same name before this one
				while (result.ContainsKey((id, name, nameIndex)))
				{
					nameIndex++;
				}
				result.Add((id, name, nameIndex), namedObject);
			}
		}
		return result;
	}
}