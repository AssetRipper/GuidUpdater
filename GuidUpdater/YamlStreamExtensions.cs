using System.IO;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;
internal static class YamlStreamExtensions
{
	public static void Save(this YamlStream yamlStream, string path)
	{
		using StreamWriter streamWriter = new StreamWriter(path);
		yamlStream.Save(streamWriter);
	}
}
