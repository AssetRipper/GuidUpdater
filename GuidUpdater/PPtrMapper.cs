using GuidUpdater.Matching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GuidUpdater;

public static class PPtrMapper
{
	private static readonly Stack<AssetMatcher> matchers = new()
	{
		new DefaultMatcher(),
		new NamedObjectMatcher(),
		new PrefabMatcher(),
		new SingletonMatcher(),
	};

	//Required for the initialization above
	private static void Add(this Stack<AssetMatcher> stack, AssetMatcher matcher) => stack.Push(matcher);

	/// <summary>
	/// Make a mapping of all the pptr's in the project
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

		AddDirectoryToMapping(oldRootDirectory, oldRootDirectory, newRootDirectory);
	}

	private static void AddDirectoryToMapping(string oldRootDirectory, string searchDirectory, string newRootDirectory)
	{
		foreach (string directory in Directory.GetDirectories(searchDirectory))
		{
			if (!FilePaths.IsIgnoredFolder(directory))
			{
				AddDirectoryToMapping(oldRootDirectory, directory, newRootDirectory);
			}
		}
		foreach (string oldMetaPath in Directory.GetFiles(searchDirectory, "*.meta", SearchOption.TopDirectoryOnly))
		{
			string newMetaPath = Path.Combine(newRootDirectory, Path.GetRelativePath(oldRootDirectory, oldMetaPath));
			if (!File.Exists(newMetaPath))
			{
				continue;
			}

			UnityGuid oldGuid = MetaFile.FromFile(oldMetaPath).Guid;
			UnityGuid newGuid = MetaFile.FromFile(newMetaPath).Guid;

			string oldAssetPath = oldMetaPath.Substring(0, oldMetaPath.Length - 5);
			string newAssetPath = newMetaPath.Substring(0, newMetaPath.Length - 5);
			if (File.Exists(oldAssetPath) && File.Exists(newAssetPath) && FilePaths.IsSerializedFileAndNotScene(oldAssetPath))
			{
				AssetFile oldAssetFile = AssetFile.FromFile(oldAssetPath);
				AssetFile newAssetFile = AssetFile.FromFile(newAssetPath);
				AssetMatcher matcher = matchers.First(m => m.Applies(oldAssetFile, newAssetFile));
				foreach ((long fileID1, long fileID2) in matcher.GetMatches(oldAssetFile, newAssetFile))
				{
					IdentifierMap.Map(new PPtr(fileID1, oldGuid, AssetType.Serialized), new PPtr(fileID2, newGuid, AssetType.Serialized));
				}
			}
		}
	}
}
