using System.Collections.Generic;

namespace GuidUpdater.Assets;

public readonly record struct Component(UnityAsset Asset, Dictionary<long, UnityAsset> FileDictionary)
{
    public bool IsTransform => Asset.ClassID is 4 or 224;
    public bool IsMonoBehaviour => Asset.ClassID is 114;
    public GameObject GameObject
    {
        get
        {
            long fileID = Asset.AssetRootNode.GetValue("m_GameObject").ParseAsPPtr().FileID;
            return new GameObject(FileDictionary[fileID], FileDictionary);
        }
    }
	public static implicit operator UnityAsset(Component asset) => asset.Asset;
}
