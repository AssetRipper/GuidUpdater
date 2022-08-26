using System;
using System.IO;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;

public static class ReferenceUpdater
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
		bool modified = false;
		MetaFile meta = MetaFile.FromFile(path);
		oldGuid = meta.Guid;

		if (IdentifierMap.TryGetNewGuid(oldGuid, out UnityGuid newGuid))
		{
			meta.Guid = newGuid;
			modified = true;
		}

		if (meta.ImporterName == "NativeFormatImporter")
		{
			YamlScalarNode fileIDNode = meta.ImporterRootNode.GetValue("mainObjectFileID").CastToScalar();
			string? fileIDString = fileIDNode.Value;
			if (!string.IsNullOrEmpty(fileIDString))
			{
				PPtr pptr = new PPtr(long.Parse(fileIDString), oldGuid, AssetType.Serialized);
				if (pptr.IsReplaceable)
				{
					if (IdentifierMap.TryGetNewPPtr(pptr.ToInterFile(oldGuid), out PPtr newPPtr))
					{
						fileIDNode.Value = newPPtr.FileID.ToString();
						modified = true;
					}
				}
			}
		}

		foreach (YamlPPtrNode node in meta.ImporterRootNode.FindAllPPtrs())
		{
			PPtr pptr = node.PPtr;
			if (pptr.IsReplaceable)
			{
				if (IdentifierMap.TryGetNewPPtr(pptr.ToInterFile(oldGuid), out PPtr newPPtr))
				{
					node.PPtr = newPPtr;
					modified = true;
				}
			}
		}

		if (modified)
		{
			meta.Stream.SaveForUnity(true, path);
		}
	}

	private static void UpdateAssetFile(string path, UnityGuid oldMetaGuid)
	{
		bool modified = false;
		AssetFile file = AssetFile.FromFile(path);
		foreach (UnityAsset asset in file.Assets)
		{
			foreach (YamlPPtrNode node in asset.FindAllPPtrs())
			{
				PPtr pptr = node.PPtr;
				if (pptr.IsReplaceable)
				{
					if (IdentifierMap.TryGetNewPPtr(pptr.ToInterFile(oldMetaGuid), out PPtr newPPtr))
					{
						node.PPtr = newPPtr;
						modified = true;
					}
				}
			}
			PPtr assetPPtr = new PPtr(asset.FileID, oldMetaGuid, AssetType.Serialized);
			if (IdentifierMap.TryGetNewPPtr(assetPPtr, out PPtr newAssetPPtr))
			{
				asset.FileID = newAssetPPtr.FileID;
				modified = true;
			}
		}
		if (modified)
		{
			file.Stream.SaveForUnity(true, path);
		}
	}
}
