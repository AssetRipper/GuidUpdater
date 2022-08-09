using System.Collections.Generic;

namespace GuidUpdater.Assets;

public readonly record struct MonoBehaviour(UnityAsset Asset, Dictionary<long, UnityAsset> FileDictionary)
{
	public GameObject GameObject
	{
		get
		{
			long fileID = Asset.AssetRootNode.GetValue("m_GameObject").ParseAsPPtr().FileID;
			return new GameObject(FileDictionary[fileID], FileDictionary);
		}
	}
	public PPtr Script => Asset.AssetRootNode.GetValue("m_Script").ParseAsPPtr().PPtr;
	public static implicit operator UnityAsset(MonoBehaviour behaviour) => behaviour.Asset;
}