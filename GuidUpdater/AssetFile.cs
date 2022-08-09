using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;
public readonly record struct AssetFile(YamlStream Stream)
{
	private static readonly Regex strippedAssetHeaderRegex = new Regex(@"(\n--- !u![0-9]+ &-?[0-9]+)( )(stripped\r?\n)", RegexOptions.Compiled);
	private const string StrippedWithSpace = " stripped";
	private const string StrippedWithUnderscore = "_stripped";

	public static implicit operator YamlStream(AssetFile meta) => meta.Stream;
	public static implicit operator AssetFile(YamlStream stream) => new AssetFile(stream);

	public static AssetFile FromFile(string path)
	{
		return FromText(File.ReadAllText(path));
	}

	public static AssetFile FromText(string input)
	{
		input = strippedAssetHeaderRegex.Replace(input, "$1_$3");
		YamlStream yaml = new();
		using StringReader reader = new StringReader(input);
		yaml.Load(reader);
		FixAnchorUnderscores(yaml.Documents);
		return yaml;
	}

	private static void FixAnchorUnderscores(IList<YamlDocument> documents)
	{
		foreach (YamlDocument document in documents)
		{
			string anchor = document.RootNode.Anchor.Value;
			if (anchor.EndsWith(StrippedWithUnderscore, StringComparison.Ordinal))
			{
				document.RootNode.Anchor = string.Concat(anchor.AsSpan(0, anchor.Length - StrippedWithUnderscore.Length), StrippedWithSpace);
			}
		}
	}

	public IEnumerable<UnityAsset> Assets => Stream.Documents.Select(document => new UnityAsset(document));
	public int Count => Stream.Documents.Count;
	public UnityAsset this[int index] => Stream.Documents[index];
	public Dictionary<long, UnityAsset> MakeAssetDictionary() => Assets.ToDictionary(asset => asset.FileID, asset => asset);
}