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
    public static explicit operator Transform(Component component) => new Transform(component.Asset, component.FileDictionary);
    public static implicit operator Component(Transform transform) => new Component(transform.Asset, transform.FileDictionary);
	public static explicit operator MonoBehaviour(Component component) => new MonoBehaviour(component.Asset, component.FileDictionary);
	public static implicit operator Component(MonoBehaviour behaviour) => new Component(behaviour.Asset, behaviour.FileDictionary);
	public static implicit operator UnityAsset(Component component) => component.Asset;
}
