using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;
public static class YamlDocumentExtensions
{
	private const string TagPrefix = "tag:unity3d.com,2011:";
	private const string StrippedSuffix = " stripped";

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

	public static int GetClassID(this YamlDocument document)
	{
		string tag = document.RootNode.Tag.Value;
		Debug.Assert(tag.StartsWith(TagPrefix, StringComparison.Ordinal), $"Tag was not prefixed correctly: {tag}");
		return int.Parse(tag.AsSpan(TagPrefix.Length));
	}

	public static void SetClassID(this YamlDocument document, int value)
	{
		document.RootNode.Tag = $"{TagPrefix}{value}";
	}

	public static long GetFileID(this YamlDocument document)
	{
		string anchor = document.RootNode.Anchor.Value;
		return anchor.EndsWith(StrippedSuffix, StringComparison.Ordinal)
			? long.Parse(anchor.AsSpan(0, anchor.Length - StrippedSuffix.Length))
			: long.Parse(anchor);
	}

	public static void SetFileID(this YamlDocument document, long value)
	{
		document.RootNode.Anchor = document.RootNode.Anchor.Value.EndsWith(StrippedSuffix, StringComparison.Ordinal)
			? $"{value}{StrippedSuffix}"
			: value.ToString();
	}
}
