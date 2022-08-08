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
	public static bool TryParseAsPPtr(this YamlNode node, out PPtr pptr)
	{
		if (node is YamlMappingNode mappingNode)
		{
			switch (mappingNode.Children.Count)
			{
				case 1:
					{
						(YamlNode key, YamlNode value) = mappingNode.Children[0];
						if (key.IsMatchingScalarNode("fileID") && value.IsScalarNode(out string? valueString) && !string.IsNullOrEmpty(valueString))
						{
							pptr = new PPtr(long.Parse(valueString));
							return true;
						}
					}
					break;
				case 3:
					{
						(YamlNode key1, YamlNode value1) = mappingNode.Children[0];
						(YamlNode key2, YamlNode value2) = mappingNode.Children[1];
						(YamlNode key3, YamlNode value3) = mappingNode.Children[2];
						if (key1.IsMatchingScalarNode("fileID") && value1.IsScalarNode(out string? valueString1) && !string.IsNullOrEmpty(valueString1)
							&& key2.IsMatchingScalarNode("guid") && value2.IsScalarNode(out string? valueString2) && !string.IsNullOrEmpty(valueString2)
							&& key3.IsMatchingScalarNode("type") && value3.IsScalarNode(out string? valueString3) && !string.IsNullOrEmpty(valueString3))
						{
							pptr = new PPtr(long.Parse(valueString1), UnityGuid.Parse(valueString2), (AssetType)byte.Parse(valueString3));
							return true;
						}
					}
					break;
			}
		}
		pptr = default;
		return false;
	}

	public static PPtr ParseAsPPtr(this YamlNode node)
	{
		return node.TryParseAsPPtr(out PPtr pptr) 
			? pptr 
			: throw new ArgumentException("Node is not a PPtr", nameof(node));
	}

	private static bool IsMatchingScalarNode(this YamlNode node, string expectedValue)
	{
		return node is YamlScalarNode scalarNode && scalarNode.Value == expectedValue;
	}

	private static bool IsScalarNode(this YamlNode node, out string? value)
	{
		if(node is YamlScalarNode scalarNode)
		{
			value = scalarNode.Value;
			return true;
		}
		else
		{
			value = null;
			return false;
		}
	}
}