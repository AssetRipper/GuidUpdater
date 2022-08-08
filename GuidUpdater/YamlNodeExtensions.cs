using System;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;
internal static class YamlNodeExtensions
{
	/// <summary>
	/// Attempt to parse a <see cref="PPtr"/> from a <see cref="YamlNode"/>.
	/// </summary>
	/// <param name="node">The node to parse.</param>
	/// <param name="pptr">The parsed pptr if successful.</param>
	/// <returns>True if the node was a PPtr.</returns>
	public static bool TryParseAsPPtr(this YamlNode node, out YamlPPtrNode pptr)
	{
		if (node is YamlMappingNode mappingNode)
		{
			switch (mappingNode.Children.Count)
			{
				case 1:
					{
						(YamlNode key, YamlNode value) = mappingNode.Children[0];
						if (key.IsMatchingScalarNode("fileID") && value.IsNonemptyScalarNode())
						{
							pptr = new YamlPPtrNode(mappingNode, true);
							return true;
						}
					}
					break;
				case 3:
					{
						(YamlNode key1, YamlNode value1) = mappingNode.Children[0];
						(YamlNode key2, YamlNode value2) = mappingNode.Children[1];
						(YamlNode key3, YamlNode value3) = mappingNode.Children[2];
						if (key1.IsMatchingScalarNode("fileID") && value1.IsNonemptyScalarNode()
							&& key2.IsMatchingScalarNode("guid") && value2.IsNonemptyScalarNode()
							&& key3.IsMatchingScalarNode("type") && value3.IsNonemptyScalarNode())
						{
							pptr = new YamlPPtrNode(mappingNode, false);
							return true;
						}
					}
					break;
			}
		}
		pptr = default;
		return false;
	}

	public static YamlPPtrNode ParseAsPPtr(this YamlNode node)
	{
		return node.TryParseAsPPtr(out YamlPPtrNode pptr) 
			? pptr 
			: throw new ArgumentException("Node is not a PPtr", nameof(node));
	}

	private static bool IsMatchingScalarNode(this YamlNode node, string expectedValue)
	{
		return node is YamlScalarNode scalarNode && scalarNode.Value == expectedValue;
	}
	
	private static bool IsNonemptyScalarNode(this YamlNode node)
	{
		return node is YamlScalarNode scalarNode && !string.IsNullOrEmpty(scalarNode.Value);
	}
}