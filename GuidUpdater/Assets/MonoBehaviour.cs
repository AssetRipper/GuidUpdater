using System.Collections.Generic;

namespace GuidUpdater.Assets;

public readonly record struct MonoBehaviour(UnityAsset Asset, Dictionary<long, UnityAsset> FileDictionary)
{
	public GameObject GameObject => ((Component)this).GameObject;
	public PPtr Script => Asset.AssetRootNode.GetValue("m_Script").ParseAsPPtr().PPtr;
	public static implicit operator UnityAsset(MonoBehaviour asset) => asset.Asset;
}