using System;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;
public static class YamlMetaParser
{
	public static UnityGuid GetGuidFromYamlStream(YamlStream stream)
	{
		YamlMappingNode rootNode = stream.Documents[0].RootNode as YamlMappingNode 
			?? throw new Exception("Root node must be a map");
		YamlScalarNode guidNode = (YamlScalarNode)rootNode.Children.Single(pair => ((YamlScalarNode)pair.Key).Value == "guid").Value;
		string guid = guidNode.Value ?? throw new Exception("Guid cannot be null");
		if (guid.Length != 32)
		{
			throw new Exception("guid must be 32 characters");
		}

		return UnityGuid.Parse(guid);
	}
}
