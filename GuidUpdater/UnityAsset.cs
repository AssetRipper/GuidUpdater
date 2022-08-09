using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;
public readonly record struct UnityAsset(YamlDocument Document)
{
	private const string TagPrefix = "tag:unity3d.com,2011:";
	private const string StrippedSuffix = " stripped";

	public int ClassID
	{
		get
		{
			string tag = Tag;
			Debug.Assert(tag.StartsWith(TagPrefix, StringComparison.Ordinal), $"Tag was not prefixed correctly: {tag}");
			return int.Parse(tag.AsSpan(TagPrefix.Length));
		}
		set
		{
			Tag = $"{TagPrefix}{value}";
		}
	}

	public long FileID
	{
		get
		{
			string anchor = Anchor;
			return anchor.EndsWith(StrippedSuffix, StringComparison.Ordinal)
				? long.Parse(anchor.AsSpan(0, anchor.Length - StrippedSuffix.Length))
				: long.Parse(anchor);
		}
		set
		{
			Anchor = Anchor.EndsWith(StrippedSuffix, StringComparison.Ordinal)
				? $"{value}{StrippedSuffix}"
				: value.ToString();
		}
	}

	public bool Stripped
	{
		get => Anchor.EndsWith(StrippedSuffix, StringComparison.Ordinal);
		set
		{
			if (value && !Stripped)
			{
				Anchor = $"{Anchor}{StrippedSuffix}";
			}
			else if (!value && Stripped)
			{
				Anchor = Anchor[..^StrippedSuffix.Length];
			}
		}
	}

	private string Anchor
	{
		get => Document.RootNode.Anchor.Value;
		set => Document.RootNode.Anchor = value;
	}

	private string Tag
	{
		get => Document.RootNode.Tag.Value;
		set => Document.RootNode.Tag = value;
	}
	
	/// <summary>
	/// The <see cref="YamlMappingNode"/> containing the top-level properties of the asset, such as m_ObjectHideFlags.
	/// </summary>
	/// <remarks>
	/// This is the child of <see cref="YamlDocument.RootNode"/>.
	/// </remarks>
	public YamlMappingNode AssetRootNode
	{
		get
		{
			Debug.Assert(Document.RootNode is YamlMappingNode);
			YamlMappingNode rootNode = (YamlMappingNode)Document.RootNode;
			Debug.Assert(rootNode.Children.Count == 1);
			return (YamlMappingNode)rootNode.Children[0].Value;
		}
	}

	public static implicit operator YamlDocument(UnityAsset asset) => asset.Document;
	public static implicit operator UnityAsset(YamlDocument document) => new UnityAsset(document);

	public bool TryParseName([NotNullWhen(true)] out string? name)
	{
		if (AssetRootNode.TryGetValue("m_Name", out YamlNode? value) && value is YamlScalarNode valueScalar)
		{
			name = valueScalar.Value ?? string.Empty;
			return true;
		}
		name = null;
		return false;
	}

	public IEnumerable<YamlPPtrNode> FindAllPPtrs() => AssetRootNode.FindAllPPtrs();
}
