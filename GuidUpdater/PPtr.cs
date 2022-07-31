namespace GuidUpdater;

public readonly record struct PPtr(long FileID, UnityGuid Guid, AssetType Type)
{
	public override string ToString()
	{
		return Guid == UnityGuid.Zero
			? $"{{fileID: {FileID}}}"
			: $"{{fileID: {FileID}, guid: {Guid}, type: {(byte)Type}}}";
	}

	public bool IsNull => FileID == 0;
}
