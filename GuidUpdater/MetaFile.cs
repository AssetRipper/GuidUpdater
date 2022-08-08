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
			YamlScalarNode guidNode = GetGuidNode(Stream);
			string guid = guidNode.Value!;
			Debug.Assert(guid != null, "Guid cannot be null");
			Debug.Assert(guid.Length == 32, "Guid must be 32 characters");
			return UnityGuid.Parse(guid);
		}
		set
		{
			YamlScalarNode guidNode = GetGuidNode(Stream);
			guidNode.Value = value.ToString();
		}
	}
	
	private static YamlScalarNode GetGuidNode(YamlStream stream)
	{
		Debug.Assert(stream.Documents.Count == 1);
		YamlMappingNode rootNode = (YamlMappingNode)stream.Documents[0].RootNode;
		return (YamlScalarNode)rootNode.Children.First(pair => ((YamlScalarNode)pair.Key).Value == "guid").Value;
	}
	
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