using System;
using System.Collections.Generic;

namespace GuidUpdater;
internal static class IdentifierMap
{
	private static Dictionary<UnityGuid, UnityGuid> GuidMap { get; } = new();
	private static Dictionary<PPtrData, PPtrData> PPtrMap { get; } = new();

	public static void Map(UnityGuid oldGuid, UnityGuid newGuid)
	{
		if (GuidMap.TryGetValue(oldGuid, out UnityGuid value))
		{
			if (value != newGuid)
			{
				throw new Exception($"Guid {oldGuid} can't be mapped to {newGuid} because it's already mapped to {value}.");
			}
		}
		else
		{
			GuidMap.Add(oldGuid, newGuid);
		}
	}

	public static void Map(PPtr oldPPtr, PPtr newPPtr)
	{
		ThrowIfInvalidForMapping(oldPPtr);
		ThrowIfInvalidForMapping(newPPtr);
		if (PPtrMap.TryGetValue(oldPPtr.ToData(), out PPtrData value))
		{
			if (value != newPPtr.ToData())
			{
				throw new Exception($"Guid {oldPPtr} can't be mapped to {newPPtr} because it's already mapped to {value}.");
			}
		}
		else
		{
			PPtrMap.Add(oldPPtr.ToData(), newPPtr.ToData());
		}
	}

	public static bool TryGetNewPPtr(PPtr oldPPtr, out PPtr newPPtr)
	{
		ThrowIfInvalidForMapping(oldPPtr);
		if (PPtrMap.TryGetValue(oldPPtr.ToData(), out PPtrData data))
		{
			newPPtr = data.ToPPtr(oldPPtr.Type);
		}
		else if (GuidMap.TryGetValue(oldPPtr.Guid, out UnityGuid newGuid))
		{
			newPPtr = new PPtr(oldPPtr.FileID, newGuid, oldPPtr.Type);
		}
		else
		{
			newPPtr = oldPPtr;
		}
		return oldPPtr != newPPtr;
	}

	public static PPtr GetNewPPtr(PPtr oldPPtr)
	{
		TryGetNewPPtr(oldPPtr, out PPtr newPPtr);
		return newPPtr;
	}

	public static bool TryGetNewGuid(UnityGuid oldGuid, out UnityGuid newGuid)
	{
		newGuid = GuidMap.TryGetValue(oldGuid, out UnityGuid value) ? value : oldGuid;
		return newGuid != oldGuid;
	}

	private static void ThrowIfInvalidForMapping(PPtr pptr)
	{
		if (pptr.IsNull || pptr.IsMissing || pptr.IsInternal || pptr.IsIntraFile)
		{
			throw new ArgumentException($"Invalid for mapping: {pptr}", nameof(pptr));
		}
	}

	private static PPtrData ToData(this PPtr pptr) => new PPtrData(pptr);

	private readonly record struct PPtrData(long FileID, UnityGuid Guid)
	{
		public PPtrData(PPtr pptr) : this(pptr.FileID, pptr.Guid) { }
		public PPtr ToPPtr(AssetType type) => new PPtr(FileID, Guid, type);
		public override string ToString() => ToPPtr(AssetType.Serialized).ToString();
	}
}
