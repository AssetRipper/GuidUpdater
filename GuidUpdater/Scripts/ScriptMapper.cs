using System.Collections.Generic;
using System.IO;

namespace GuidUpdater.Scripts;
/// <summary>
/// Handler for scripts without a direct mapping.
/// </summary>
public static class ScriptMapper
{
	private const int MonoScriptMainID = 115 * 100000;
	public static readonly Dictionary<UnityGuid, FullName> oldScriptGuids = new();
	public static readonly Dictionary<UnityGuid, Dictionary<long, FullName>> oldAssemblyGuids = new();
	public static readonly Dictionary<FullName, UnityGuid> newScriptGuids = new();
	public static readonly Dictionary<FullName, PPtr> newAssemblyGuids = new();

	private static PPtr ToScriptPPtr(UnityGuid scriptGuid)
	{
		return new PPtr(MonoScriptMainID, scriptGuid, AssetType.Meta);
	}

	public static void RegisterOldScript(UnityGuid guid, string path)
	{
		string name = Path.GetFileNameWithoutExtension(path) ?? throw new System.Exception();
		if (ScriptParser.TryFindPossibleMonoBehaviour(File.ReadAllText(path), name, out string? @namespace))
		{
			oldScriptGuids.Add(guid, new FullName(@namespace, name));
		}
	}

	public static void RegisterNewScript(UnityGuid guid, string path)
	{
		string name = Path.GetFileNameWithoutExtension(path) ?? throw new System.Exception();
		if (ScriptParser.TryFindPossibleMonoBehaviour(File.ReadAllText(path), name, out string? @namespace))
		{
			newScriptGuids.Add(new FullName(@namespace, name), guid);
		}
	}

	public static void RegisterOldAssembly(UnityGuid guid, string path)
	{
		if (!oldAssemblyGuids.TryGetValue(guid, out Dictionary<long, FullName>? dictionary))
		{
			dictionary = new Dictionary<long, FullName>();
			oldAssemblyGuids.Add(guid, dictionary);
		}
		foreach (FullName fullName in AssemblyParser.GetPotentialMonoBehavioursFromAssembly(path))
		{
			int fileID = AssemblyParser.ComputeFileID(fullName);
			dictionary.Add(fileID, fullName);
		}
	}

	public static void RegisterNewAssembly(UnityGuid guid, string path)
	{
		foreach (FullName fullName in AssemblyParser.GetPotentialMonoBehavioursFromAssembly(path))
		{
			int fileID = AssemblyParser.ComputeFileID(fullName);
			newAssemblyGuids.Add(fullName, new PPtr(fileID, guid, AssetType.Meta));
		}
	}

	public static void BuildMapping()
	{
		foreach ((UnityGuid guid, FullName fullName) in oldScriptGuids)
		{
			if (newScriptGuids.TryGetValue(fullName, out UnityGuid newGuid))
			{
				IdentifierMap.Map(ToScriptPPtr(guid), ToScriptPPtr(newGuid));
			}
			else if (newAssemblyGuids.TryGetValue(fullName, out PPtr newPPtr))
			{
				IdentifierMap.Map(ToScriptPPtr(guid), newPPtr);
			}
		}
		foreach ((UnityGuid guid, Dictionary<long, FullName> dictionary) in oldAssemblyGuids)
		{
			foreach ((long fileID, FullName fullName) in dictionary)
			{
				PPtr pptr = new PPtr(fileID, guid, AssetType.Meta);
				if (newScriptGuids.TryGetValue(fullName, out UnityGuid newGuid))
				{
					IdentifierMap.Map(pptr, ToScriptPPtr(newGuid));
				}
				else if (newAssemblyGuids.TryGetValue(fullName, out PPtr newPPtr))
				{
					IdentifierMap.Map(pptr, newPPtr);
				}
			}
		}
	}
}
