using System;
using System.Diagnostics;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;
public static class YamlMetaParser
{
	public static UnityGuid GetGuidFromYamlStream(YamlStream stream)
	{
		YamlScalarNode guidNode = GetGuidNode(stream);
		string guid = guidNode.Value!;
		Debug.Assert(guid != null, "Guid cannot be null");
		Debug.Assert(guid.Length == 32, "Guid must be 32 characters");
		return UnityGuid.Parse(guid);
	}

	public static void SetGuidInYamlStream(YamlStream stream, UnityGuid guid)
	{
		YamlScalarNode guidNode = GetGuidNode(stream);
		guidNode.Value = guid.ToString();
	}

	private static YamlScalarNode GetGuidNode(YamlStream stream)
	{
		Debug.Assert(stream.Documents.Count == 1);
		YamlMappingNode rootNode = (YamlMappingNode)stream.Documents[0].RootNode;
		return (YamlScalarNode)rootNode.Children.First(pair => ((YamlScalarNode)pair.Key).Value == "guid").Value;
	}
}
