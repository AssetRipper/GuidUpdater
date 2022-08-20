using System;
using System.Collections.Generic;
using System.IO;

namespace GuidUpdater;

public static class PathMapper
{
	private static readonly HashSet<string> oldPaths = new();
	private static readonly HashSet<string> commonPaths = new();
	private static readonly HashSet<string> newPaths = new();

	public static string OldRootDirectory { get; private set; } = "";
	public static string NewRootDirectory { get; private set; } = "";
	public static IEnumerable<string> OldPaths => oldPaths;
	public static IEnumerable<string> CommonPaths => commonPaths;
	public static IEnumerable<string> NewPaths => newPaths;

	/// <summary>
	/// Make a mapping of all the relative paths in the project
	/// </summary>
	/// <param name="oldRootDirectory">the absolute path to the old root directory, ie the assets folder</param>
	/// <param name="newRootDirectory">the absolute path to the new root directory, ie the assets folder</param>
	/// <exception cref="ArgumentException">Directory doesn't exist</exception>
	public static void Map(string oldRootDirectory, string newRootDirectory)
	{
		if (!Directory.Exists(oldRootDirectory))
		{
			throw new ArgumentException(null, nameof(oldRootDirectory));
		}
		if (!Directory.Exists(newRootDirectory))
		{
			throw new ArgumentException(null, nameof(newRootDirectory));
		}

		OldRootDirectory = oldRootDirectory;
		NewRootDirectory = newRootDirectory;

		AddDirectoryToMapping(oldRootDirectory, oldRootDirectory, newRootDirectory, oldPaths);
		AddDirectoryToMapping(newRootDirectory, newRootDirectory, oldRootDirectory, newPaths);
	}

	private static void AddDirectoryToMapping(string primaryRootDirectory, string searchDirectory, string secondaryRootDirectory, HashSet<string> primarySet)
	{
		foreach (string directory in Directory.GetDirectories(searchDirectory))
		{
			if (!FilePaths.IsIgnoredFolder(directory))
			{
				AddDirectoryToMapping(primaryRootDirectory, directory, secondaryRootDirectory, primarySet);
			}
		}
		foreach (string primaryMetaPath in Directory.GetFiles(searchDirectory, "*.meta", SearchOption.TopDirectoryOnly))
		{
			string primaryAssetPath = primaryMetaPath.Substring(0, primaryMetaPath.Length - 5);
			if (!File.Exists(primaryAssetPath) && !Directory.Exists(primaryAssetPath))
			{
				throw new Exception($"Loose meta file: {primaryMetaPath}");
			}

			string relativePath = Path.GetRelativePath(primaryRootDirectory, primaryMetaPath);
			string secondaryMetaPath = Path.Combine(secondaryRootDirectory, relativePath);
			if (File.Exists(secondaryMetaPath))
			{
				string secondaryAssetPath = secondaryMetaPath.Substring(0, secondaryMetaPath.Length - 5);
				if (!File.Exists(secondaryAssetPath) && !Directory.Exists(secondaryAssetPath))
				{
					throw new Exception($"Loose meta file: {secondaryAssetPath}");
				}
				commonPaths.Add(relativePath);
			}
			else
			{
				primarySet.Add(relativePath);
			}
		}
	}
}