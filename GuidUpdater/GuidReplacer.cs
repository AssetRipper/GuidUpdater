using System;
using System.IO;

namespace GuidUpdater;

public static class GuidReplacer
{	
	/// <summary>
	/// Update all references in the project
	/// </summary>
	/// <param name="searchDirectory">The absolute path to a directory with files containing references.</param>
	/// <exception cref="ArgumentException">Directory doesn't exist</exception>
	public static void UpdateReferencesInDirectory(string searchDirectory)
	{
		foreach (string directory in Directory.GetDirectories(searchDirectory))
		{
			if (!FilePaths.IsIgnoredFolder(directory))
			{
				UpdateReferencesInDirectory(directory);
			}
		}
		foreach (string oldMetaPath in Directory.GetFiles(searchDirectory, "*.meta", SearchOption.TopDirectoryOnly))
		{
			UpdateMetaFile(oldMetaPath, out UnityGuid oldGuid);

			string oldAssetPath = oldMetaPath.Substring(0, oldMetaPath.Length - 5);
			if (File.Exists(oldAssetPath) && FilePaths.IsSerializedFile(oldAssetPath))
			{
				UpdateAssetFile(oldAssetPath, oldGuid);
			}
		}
	}

	private static void UpdateMetaFile(string path, out UnityGuid oldGuid)
	{
		MetaFile meta = MetaFile.FromFile(path);
		oldGuid = meta.Guid;

		if (IdentifierMap.TryGetNewGuid(oldGuid, out UnityGuid newGuid))
		{
			meta.Guid = newGuid;
		}
		//todo: importer pptrs
		meta.Stream.SaveForUnity(path);
	}

	private static void UpdateAssetFile(string path, UnityGuid oldMetaGuid)
	{
		AssetFile file = AssetFile.FromFile(path);
		foreach (UnityAsset asset in file.Assets)
		{
			foreach (YamlPPtrNode node in asset.FindAllPPtrs())
			{
				PPtr pptr = node.PPtr;
				if (pptr.IsReplaceable)
				{
					node.PPtr = IdentifierMap.GetNewPPtr(pptr.ToInterFile(oldMetaGuid));
				}
			}
			PPtr assetPPtr = new PPtr(asset.FileID, oldMetaGuid, AssetType.Serialized);
			asset.FileID = IdentifierMap.GetNewPPtr(assetPPtr).FileID;
		}
	}
}
