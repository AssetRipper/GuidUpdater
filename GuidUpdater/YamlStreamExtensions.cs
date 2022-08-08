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

	public static void SaveForUnity(this YamlStream yamlStream, bool assetFile, string path)
	{
		File.WriteAllText(path, yamlStream.SaveForUnity(assetFile));
	}

	public static string SaveForUnity(this YamlStream yamlStream, bool assetFile)
	{
		EmitterPatch.EmittingAssetFile = assetFile;
		using StringWriter writer = new();
		writer.NewLine = "\n";
		Emitter emitter = new Emitter(writer);
		yamlStream.Save(emitter, false);
		return writer.ToString();
	}
}
