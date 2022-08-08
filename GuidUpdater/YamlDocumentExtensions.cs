using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;
public static class YamlDocumentExtensions
{
	public static bool TryParseName(this YamlDocument document, [NotNullWhen(true)] out string? name)
	{
		YamlMappingNode mappingNode = document.GetAssetPropertyMappingNode();
		if (mappingNode.TryGetValue("m_Name", out YamlNode? value) && value is YamlScalarNode valueScalar)
		{
			name = valueScalar.Value ?? string.Empty;
			return true;
		}
		name = null;
		return false;
	}

	public static YamlMappingNode GetAssetPropertyMappingNode(this YamlDocument document)
	{
		Debug.Assert(document.RootNode is YamlMappingNode);
		YamlMappingNode rootNode = (YamlMappingNode)document.RootNode;
		Debug.Assert(rootNode.Children.Count == 1);
		return (YamlMappingNode)rootNode.Children[0].Value;
	}
}
