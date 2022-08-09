using System.Collections.Generic;

namespace GuidUpdater.Assets;

public readonly record struct AnimatorTransition(UnityAsset Asset, Dictionary<long, UnityAsset> FileDictionary)
{
	public string? Name => ((NamedObject)this).Name;
	public NamedObject DestinationState
	{
		get
		{
			long fileID = Asset.AssetRootNode.GetValue("m_DstState").ParseAsPPtr().FileID;
			return new GameObject(FileDictionary[fileID], FileDictionary);
		}
	}
	/// <summary>
	/// The name for transitions is often null. This uses the destionation state name as a fallback.
	/// </summary>
	public string? Identifier => Name ?? DestinationState.Name;

	public static implicit operator UnityAsset(AnimatorTransition asset) => asset.Asset;
	public static explicit operator AnimatorTransition(NamedObject asset) => new AnimatorTransition(asset.Asset, asset.FileDictionary);
	public static implicit operator NamedObject(AnimatorTransition asset) => new NamedObject(asset.Asset, asset.FileDictionary);
}