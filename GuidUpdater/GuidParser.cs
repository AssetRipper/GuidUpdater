using System;
using System.IO;

namespace GuidUpdater;

public static class GuidParser
{
	/// <summary>
	/// Make a mapping of all the guid's in the project
	/// </summary>
	/// <param name="oldRootDirectory">the absolute path to the old root directory, ie the assets folder</param>
	/// <param name="newRootDirectory">the absolute path to the new root directory, ie the assets folder</param>
	/// <exception cref="ArgumentException">Directory doesn't exist</exception>
	public static void MakeMapping(string oldRootDirectory, string newRootDirectory)
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
			string newAssetPath = oldMetaPath.Substring(0, newMetaPath.Length - 5);
			if (File.Exists(oldAssetPath) && File.Exists(newAssetPath))
			{
				if (FilePaths.IsSerializedFile(oldAssetPath))
				{
					AssetFile oldAssetFile = AssetFile.FromFile(oldAssetPath);
					AssetFile newAssetFile = AssetFile.FromFile(newAssetPath);
					//Todo: match file id's
				}
				IdentifierMap.Map(oldGuid, newGuid);
			}
			else if (!File.Exists(oldAssetPath) && !File.Exists(newAssetPath))
			{
				if (!Directory.Exists(oldAssetPath))
				{
					throw new Exception($"Loose meta file: {oldMetaPath}");
				}
				else if (!Directory.Exists(newAssetPath))
				{
					throw new Exception($"Loose meta file: {oldMetaPath}");
				}
				else
				{
					IdentifierMap.Map(oldGuid, newGuid);
				}
			}
		}
	}
}
