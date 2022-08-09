using System.Diagnostics;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;
public readonly record struct MetaFile(YamlStream Stream)
{
	public UnityGuid Guid
	{
		get
		{
			string guid = GuidNode.Value!;
			Debug.Assert(guid != null, "Guid cannot be null");
			Debug.Assert(guid.Length == 32, "Guid must be 32 characters");
			return UnityGuid.Parse(guid);
		}
		set
		{
			GuidNode.Value = value.ToString();
		}
	}

	public string? ImporterName
	{
		get
		{
			YamlMappingNode rootNode = RootNode;
			return rootNode.Children[rootNode.Children.Count - 1].Key.CastToScalar().Value;
		}
	}

	public YamlMappingNode ImporterRootNode
	{
		get
		{
			YamlMappingNode rootNode = RootNode;
			return (YamlMappingNode)rootNode.Children[rootNode.Children.Count - 1].Value;
		}
	}

	private YamlMappingNode RootNode
	{
		get
		{
			Debug.Assert(Stream.Documents.Count == 1);
			return (YamlMappingNode)Stream.Documents[0].RootNode;
		}
	}

	private YamlScalarNode GuidNode => (YamlScalarNode)RootNode.Children.First(pair => ((YamlScalarNode)pair.Key).Value == "guid").Value;
	

	public static implicit operator YamlStream(MetaFile meta) => meta.Stream;
	public static implicit operator MetaFile(YamlStream stream) => new MetaFile(stream);

	public static MetaFile FromFile(string path)
	{
		return FromText(File.ReadAllText(path));
	}

	public static MetaFile FromText(string input)
	{
		YamlStream yaml = new();
		using StringReader reader = new StringReader(input);
		yaml.Load(reader);
		return yaml;
	}
}