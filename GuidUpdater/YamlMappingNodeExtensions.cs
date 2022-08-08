using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;

public static class YamlMappingNodeExtensions
{
	public static bool TryGetValue(this YamlMappingNode node, string key, [NotNullWhen(true)] out YamlNode? value)
	{
		return node.Children.TryGetValue(key, out value);
	}

	public static YamlNode GetValue(this YamlMappingNode node, string key)
	{
		node.Children.TryGetValue(key, out YamlNode? value);
		return value ?? throw new KeyNotFoundException();
	}

	public static KeyValuePair<YamlScalarNode, YamlNode> GetChild(this YamlMappingNode node, int index)
	{
		KeyValuePair<YamlNode, YamlNode> child = node.Children[index];
		return new KeyValuePair<YamlScalarNode, YamlNode>((YamlScalarNode)child.Key, child.Value);
	}
}