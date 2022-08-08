using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;
public static class YamlLoader
{
	private static readonly Regex strippedAssetHeaderRegex = new Regex(@"(\n--- !u![0-9]+ &-?[0-9]+)( )(stripped\r?\n)", RegexOptions.Compiled);
	private const string StrippedWithSpace = " stripped";
	private const string StrippedWithUnderscore = "_stripped";

	public static YamlStream LoadAssetYamlStreamFromFile(string path)
	{
		return LoadAssetYamlStreamFromText(File.ReadAllText(path));
	}

	public static YamlStream LoadAssetYamlStreamFromText(string input)
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

	public static YamlStream LoadMetaYamlStreamFromFile(string path)
	{
		return LoadMetaYamlStreamFromText(File.ReadAllText(path));
	}

	public static YamlStream LoadMetaYamlStreamFromText(string input)
	{
		YamlStream yaml = new();
		using StringReader reader = new StringReader(input);
		yaml.Load(reader);
		return yaml;
	}
}
