using GuidUpdater.Scripts;
using System;
using System.IO;

namespace GuidUpdater;

public static class GuidMapper
{
	public static void Map()
	{
		foreach (string commonRelativePath in PathMapper.CommonPaths)
		{
			UnityGuid oldGuid = GetGuidFromPath(PathMapper.OldRootDirectory, commonRelativePath);
			UnityGuid newGuid = GetGuidFromPath(PathMapper.NewRootDirectory, commonRelativePath);
			IdentifierMap.Map(oldGuid, newGuid);
		}
		foreach(string oldRelativePath in PathMapper.OldPaths)
		{
			if (oldRelativePath.EndsWith(".cs.meta", StringComparison.InvariantCulture))
			{
				string metaPath = Path.Combine(PathMapper.OldRootDirectory, oldRelativePath);
				UnityGuid oldGuid = MetaFile.FromFile(metaPath).Guid;
				string assetPath = metaPath.Substring(0, metaPath.Length - 5);
				ScriptMapper.RegisterOldScript(oldGuid, assetPath);
			}
			else if (oldRelativePath.EndsWith(".dll.meta", StringComparison.InvariantCulture))
			{
				string metaPath = Path.Combine(PathMapper.OldRootDirectory, oldRelativePath);
				UnityGuid oldGuid = MetaFile.FromFile(metaPath).Guid;
				string assetPath = metaPath.Substring(0, metaPath.Length - 5);
				ScriptMapper.RegisterOldAssembly(oldGuid, assetPath);
			}
		}
		foreach (string newRelativePath in PathMapper.NewPaths)
		{
			if (newRelativePath.EndsWith(".cs.meta", StringComparison.InvariantCulture))
			{
				string metaPath = Path.Combine(PathMapper.NewRootDirectory, newRelativePath);
				UnityGuid guid = MetaFile.FromFile(metaPath).Guid;
				string assetPath = metaPath.Substring(0, metaPath.Length - 5);
				ScriptMapper.RegisterNewScript(guid, assetPath);
			}
			else if (newRelativePath.EndsWith(".dll.meta", StringComparison.InvariantCulture))
			{
				string metaPath = Path.Combine(PathMapper.NewRootDirectory, newRelativePath);
				UnityGuid guid = MetaFile.FromFile(metaPath).Guid;
				string assetPath = metaPath.Substring(0, metaPath.Length - 5);
				ScriptMapper.RegisterNewAssembly(guid, assetPath);
			}
		}
	}

	private static UnityGuid GetGuidFromPath(string rootDirectory, string relativePath)
	{
		return MetaFile.FromFile(Path.Combine(rootDirectory, relativePath)).Guid;
	}
}