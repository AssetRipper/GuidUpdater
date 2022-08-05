using System.IO;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;
public static class YamlLoader
{
	public static YamlStream LoadMetaYamlStreamFromFile(string path)
	{
		return LoadMetaYamlStreamFromText(File.ReadAllText(path));
	}

	public static YamlStream LoadMetaYamlStreamFromText(string input)
	{
		YamlStream yaml = new();
		using MemoryStream inputStream = new MemoryStream(Encoding.UTF8.GetBytes(input));
		using StreamReader reader = new StreamReader(inputStream, Encoding.UTF8);
		yaml.Load(reader);
		return yaml;
	}
}
