using System.Collections.Generic;

namespace GuidUpdater.Assets;

public readonly record struct NamedObject(UnityAsset Asset, Dictionary<long, UnityAsset> FileDictionary)
{
	public bool IsGameObject => Asset.ClassID is 1;
	public string? Name => Asset.AssetRootNode.GetValue("m_Name").CastToScalar().Value;
	
	public static implicit operator UnityAsset(NamedObject asset) => asset.Asset;
	public static explicit operator GameObject(NamedObject asset) => new GameObject(asset.Asset, asset.FileDictionary);
	public static implicit operator NamedObject(GameObject asset) => new NamedObject(asset.Asset, asset.FileDictionary);
}