﻿using System;
using System.Diagnostics;
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

	public static implicit operator YamlDocument(UnityAsset asset) => asset.Document;
	public static implicit operator UnityAsset(YamlDocument document) => new UnityAsset(document);
}