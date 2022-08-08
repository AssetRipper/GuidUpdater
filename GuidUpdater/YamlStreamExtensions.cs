using System.IO;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;
public static class YamlStreamExtensions
{
	static YamlStreamExtensions()
	{
		EmitterPatch.Apply();
	}

	public static void SaveForUnity(this YamlStream yamlStream, string path)
	{
		File.WriteAllText(path, yamlStream.SaveForUnity());
	}

	public static string SaveForUnity(this YamlStream yamlStream)
	{
		using StringWriter writer = new();
		Emitter emitter = new Emitter(writer);
		yamlStream.Save(emitter, false);
		string text = writer.ToString();
		if (text.EndsWith("...\r\n", System.StringComparison.Ordinal))
		{
			return text[..^5];
		}
		else if (text.EndsWith("...\n", System.StringComparison.Ordinal))
		{
			return text[..^4];
		}
		else
		{
			return text;
		}
	}
}
