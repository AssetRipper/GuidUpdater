using System.Collections.Generic;
using System.Linq;
using GuidUpdater.Assets;
using ChildDictionary = System.Collections.Generic.Dictionary<(string?, int), GuidUpdater.Assets.GameObject>;
using ComponentDictionary = System.Collections.Generic.Dictionary<(int, int), GuidUpdater.Assets.Component>;
using MonoBehaviourDictionary = System.Collections.Generic.Dictionary<(GuidUpdater.PPtr, int), GuidUpdater.Assets.MonoBehaviour>;

namespace GuidUpdater.Matching;

public class PrefabMatcher : AssetMatcher
{
	public override bool Applies(AssetFile file1, AssetFile file2)
	{
		return file1.Assets.Any(asset => asset.ClassID == 1) && file2.Assets.Any(asset => asset.ClassID == 1);
	}

	public override IEnumerable<KeyValuePair<long, long>> GetMatches(AssetFile file1, AssetFile file2)
	{
		GameObject root1 = GetRootGameObject(file1);
		GameObject root2 = GetRootGameObject(file2);
		return root1.Name == root2.Name 
			? GetMatches(root1, root2) 
			: Enumerable.Empty<KeyValuePair<long, long>>();
	}

	private static GameObject GetRootGameObject(AssetFile file)
	{
		Dictionary<long, UnityAsset> dictionary = file.MakeAssetDictionary();
		GameObject firstGameObject = new GameObject(file.Assets.First(asset => asset.ClassID == 1), dictionary);
		return firstGameObject.GetRoot();
	}

	private static IEnumerable<KeyValuePair<long, long>> GetMatches(GameObject root1, GameObject root2)
	{
		yield return new KeyValuePair<long, long>(root1.Asset.FileID, root2.Asset.FileID);

		MakeComponentDictionaries(root1, out ComponentDictionary componentDictionary1, out MonoBehaviourDictionary monoBehaviourDictionary1);
		MakeComponentDictionaries(root2, out ComponentDictionary componentDictionary2, out MonoBehaviourDictionary monoBehaviourDictionary2);
		
		foreach (((int, int) key, Component component1) in componentDictionary1)
		{
			if (componentDictionary2.TryGetValue(key, out Component component2))
			{
				yield return new KeyValuePair<long, long>(component1.Asset.FileID, component2.Asset.FileID);
			}
		}

		foreach (((PPtr, int) key, MonoBehaviour monoBehaviour1) in monoBehaviourDictionary1)
		{
			if (monoBehaviourDictionary2.TryGetValue(key, out MonoBehaviour monoBehaviour2))
			{
				yield return new KeyValuePair<long, long>(monoBehaviour1.Asset.FileID, monoBehaviour2.Asset.FileID);
			}
		}

		ChildDictionary childDictionary1 = MakeChildDictionary(root1);
		ChildDictionary childDictionary2 = MakeChildDictionary(root2);
		foreach (((string?, int) key, GameObject child1) in childDictionary1)
		{
			if (childDictionary2.TryGetValue(key, out GameObject child2))
			{
				foreach (KeyValuePair<long, long> match in GetMatches(child1, child2))
				{
					yield return match;
				}
			}
		}
	}

	private static ChildDictionary MakeChildDictionary(GameObject gameObject)
	{
		ChildDictionary result = new();
		foreach (GameObject child in gameObject.Children)
		{
			string? name = child.Name;
			int nameIndex = 0;//The number of siblings with the same name before this one
			while(result.ContainsKey((name, nameIndex)))
			{
				nameIndex++;
			}
			result.Add((name, nameIndex), child);
		}
		return result;
	}

	private static void MakeComponentDictionaries(GameObject gameObject, 
		out ComponentDictionary componentDictionary, 
		out MonoBehaviourDictionary monoBehaviourDictionary)
	{
		componentDictionary = new();
		monoBehaviourDictionary = new();
		foreach (Component component in gameObject.Components)
		{
			if (component.IsMonoBehaviour)
			{
				MonoBehaviour monoBehaviour = (MonoBehaviour)component;
				PPtr script = IdentifierMap.GetNewPPtr(monoBehaviour.Script);
				int scriptIndex = 0;//The number of monobehaviours with the same script before this one
				while (monoBehaviourDictionary.ContainsKey((script, scriptIndex)))
				{
					scriptIndex++;
				}
				monoBehaviourDictionary.Add((script, scriptIndex), monoBehaviour);
			}
			else
			{
				int id = component.Asset.ClassID;
				int idIndex = 0;//The number of components with the same id before this one
				while (componentDictionary.ContainsKey((id, idIndex)))
				{
					idIndex++;
				}
				componentDictionary.Add((id, idIndex), component);
			}
		}
	}
}
