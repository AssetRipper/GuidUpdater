using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;

public static class GuidParser
{
	static readonly string[] ignoredFolders = new string[] { "CombinedMesh", "Scenes" };
	static readonly bool ignoreDirectoryMetaFiles = true;


	internal static void SaveGuidMap(string projectDirectory, string savePath)
	{
		Dictionary<string, string> guidMap = MakeGuidMap(projectDirectory);
		Console.WriteLine("Finished making guid map");
		JsonSerializerOptions options = new JsonSerializerOptions();
		options.WriteIndented = true;
		string jsonText = JsonSerializer.Serialize(guidMap, options);
		string saveDirectory = Path.GetDirectoryName(Path.GetFullPath(savePath)) ?? throw new Exception();
		Directory.CreateDirectory(saveDirectory);
		File.WriteAllText(savePath, jsonText);
	}

	/// <summary>
	/// Make a mapping of all the guid's in the project
	/// </summary>
	/// <param name="directoryPath">the absolute path to the root directory, ie the assets folder</param>
	/// <returns>A map of relative path : guid string</returns>
	/// <exception cref="ArgumentException">Directory doesn't exist</exception>
	internal static Dictionary<string, string> MakeGuidMap(string directoryPath)
	{
		if (!Directory.Exists(directoryPath))
		{
			throw new ArgumentException(nameof(directoryPath));
		}

		var guidMap = new Dictionary<string, string>();
		AddDirectoryToGuidMap(directoryPath, directoryPath, guidMap);
		return guidMap;
	}

	private static void AddDirectoryToGuidMap(string rootDirectory, string searchDirectory, Dictionary<string, string> map)
	{
		foreach (string directory in Directory.GetDirectories(searchDirectory))
		{
			if (!ignoredFolders.Contains(Path.GetFileName(directory)))
			{
				AddDirectoryToGuidMap(rootDirectory, directory, map);
			}
		}
		foreach (string file in Directory.GetFiles(searchDirectory, "*.meta", SearchOption.TopDirectoryOnly))
		{
			if (ignoreDirectoryMetaFiles)
			{
				string assetPath = file.Substring(0, file.Length - 5);
				if (!File.Exists(assetPath))
				{
					continue;
				}
			}
			string guid = GetGuid(file);
			string relativePath = Path.GetRelativePath(rootDirectory, file);
			map.Add(relativePath, guid);
		}
	}

	private static string GetGuid(string metaPath)
	{
		YamlDocument document = GetYamlDocuments(metaPath).Single();
		YamlMappingNode rootNode = document.RootNode as YamlMappingNode ?? throw new Exception("Root node must be a map");
		YamlScalarNode guidNode = (YamlScalarNode)rootNode.Children.Single(pair => ((YamlScalarNode)pair.Key).Value == "guid").Value;
		string guid = guidNode.Value ?? throw new Exception("Guid cannot be null");
		if (guid.Length != 32)
		{
			throw new Exception("guid must be 32 characters");
		}

		return guid;
	}

	private static IList<YamlDocument> GetYamlDocuments(string metaPath)
	{
		var yaml = new YamlStream();
		using var fs = new StreamReader(metaPath);
		yaml.Load(fs);
		return yaml.Documents;
	}
}
