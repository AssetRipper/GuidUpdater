using System.Collections.Generic;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater.Assets;

public readonly record struct Transform(UnityAsset Asset, Dictionary<long, UnityAsset> FileDictionary)
{
    public GameObject GameObject => ((Component)this).GameObject;
    public Transform? Parent
    {
        get
        {
            long fileID = Asset.AssetRootNode.GetValue("m_Father").ParseAsPPtr().FileID;
            return fileID != 0 //&& FileDictionary.TryGetValue(fileID, out UnityAsset parent) 
                ? new Transform(FileDictionary[fileID], FileDictionary)
                : null;
        }
    }
    public IEnumerable<Transform> Children
    {
        get
        {
            Dictionary<long, UnityAsset> dictionary = FileDictionary;
            YamlSequenceNode sequenceNode = Asset.AssetRootNode.GetValue("m_Children").CastToSequence();
            return sequenceNode.Children
                .Select(child => child.ParseAsPPtr().FileID)
                //.Where(id => dictionary.ContainsKey(id))
                .Select(id => new Transform(dictionary[id], dictionary));
        }
    }
    public static implicit operator UnityAsset(Transform asset) => asset.Asset;
	public static explicit operator Transform(Component asset) => new Transform(asset.Asset, asset.FileDictionary);
	public static implicit operator Component(Transform asset) => new Component(asset.Asset, asset.FileDictionary);
}
