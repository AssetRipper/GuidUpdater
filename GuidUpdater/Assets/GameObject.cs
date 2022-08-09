using System.Collections.Generic;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater.Assets;

public readonly record struct GameObject(UnityAsset Asset, Dictionary<long, UnityAsset> FileDictionary)
{
	public string? Name => ((NamedObject)this).Name;
	public IEnumerable<Component> Components
	{
		get
		{
			Dictionary<long, UnityAsset> dictionary = FileDictionary;
			YamlSequenceNode sequenceNode = Asset.AssetRootNode.GetValue("m_Component").CastToSequence();
			return sequenceNode.Children
				.Select(child => child.CastToMap().GetValue("component").ParseAsPPtr().FileID)
				//.Where(id => dictionary.ContainsKey(id))
				.Select(id => new Component(dictionary[id], dictionary));
		}
	}
	public Transform Transform => (Transform)Components.First(component => component.IsTransform);
	public GameObject? Parent => Transform.Parent?.GameObject;
	public IEnumerable<GameObject> Children => Transform.Children.Select(childTransform => childTransform.GameObject);
	public GameObject GetRoot()
	{
		GameObject root;
		GameObject? parent = this;
		do
		{
			root = (GameObject)parent;
			parent = Parent;
		} while (parent != null);
		return root;
	}
	public static implicit operator UnityAsset(GameObject asset) => asset.Asset;
	public static explicit operator GameObject(NamedObject asset) => new GameObject(asset.Asset, asset.FileDictionary);
	public static implicit operator NamedObject(GameObject asset) => new NamedObject(asset.Asset, asset.FileDictionary);
}
