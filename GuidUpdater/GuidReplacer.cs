using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GuidUpdater;

public static class GuidReplacer
{
	private readonly static string[] ignoredFileExtensions = new string[] { ".cs", ".dll", ".shader", ".png", ".obj", ".fbx", ".blend", ".unity3d", ".otf", ".ttf", ".bytes", ".txt", ".json" };
	public static void ReplaceGuids(string oldAssetsPath, Dictionary<string, string> conversionMap)
	{
		ConcurrentDictionary<string, string> concurrentConversion = new ConcurrentDictionary<string, string>(conversionMap);
		List<Task> tasks = new List<Task>();
		foreach (string file in Directory.GetFiles(oldAssetsPath, "*", SearchOption.AllDirectories))
		{
			if (IsSerializedFile(file))
			{
				tasks.Add(Task.Run(() => ReplaceOldGuidsWithNewGuids(file, concurrentConversion)));
			}
		}
		int count = tasks.Count;
		for (int i = 0; i < count; i++)
		{
			if (i % 100 == 0)
			{
				Console.WriteLine($"{i}/{count} files converted");
			}
			tasks[i].Wait();
		}
	}

	private static bool IsSerializedFile(string path)
	{
		string extension = Path.GetExtension(path);
		return !ignoredFileExtensions.Contains(extension);
	}

	private static void ReplaceOldGuidsWithNewGuids(string path, ConcurrentDictionary<string, string> conversionMap)
	{
		bool changed = false;
		string fileText = File.ReadAllText(path);

		if (HasGuid(fileText))
		{
			foreach ((string oldGuid, string newGuid) in conversionMap)
			{
				int index = fileText.IndexOf(oldGuid, StringComparison.Ordinal);
				if (index != -1)
				{
					changed = true;
					fileText = fileText.Replace(oldGuid, newGuid, StringComparison.Ordinal);
				}
			}
		}

		if (changed)
		{
			File.WriteAllText(path, fileText);
		}
	}

	private static bool HasGuid(string text)
	{
		return text.IndexOf("guid: ", StringComparison.Ordinal) != -1;
	}
}
