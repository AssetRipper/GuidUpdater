﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

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

	private static bool TryReplaceMetaGuid(string path, out UnityGuid oldGuid, out UnityGuid newGuid)
	{
		MetaFile meta = MetaFile.FromFile(path);
		oldGuid = meta.Guid;

		if (IdentifierMap.TryGetNewGuid(oldGuid, out newGuid))
		{
			meta.Guid = newGuid;
			meta.Stream.Save(path);
			return true;
		}
		else
		{
			return false;
		}
		//todo: importer pptrs
	}

	private static void ReplaceAssetPPtrs(string path, UnityGuid oldMetaGuid)
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

	private static bool HasGuid(string text)
	{
		return text.IndexOf("guid: ", StringComparison.Ordinal) != -1;
	}
}
