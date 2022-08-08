namespace GuidUpdater;

public readonly record struct PPtr(long FileID, UnityGuid Guid, AssetType Type)
{
	public PPtr(long FileID) : this(FileID, UnityGuid.Zero, AssetType.Serialized) { }
	public PPtr(long FileID, AssetType Type) : this(FileID, UnityGuid.Zero, Type) { }

	public override string ToString()
	{
		return IsIntraFile
			? $"{{fileID: {FileID}}}"
			: $"{{fileID: {FileID}, guid: {Guid}, type: {(byte)Type}}}";
	}

	public PPtr ToIntraFile()
	{
		return new PPtr(FileID, Type);
	}

	public PPtr ToInterFile(UnityGuid guid)
	{
		return IsIntraFile
			? new PPtr(FileID, guid, Type)
			: this;
	}

	/// <summary>
	/// The <see cref="PPtr"/> references nothing.
	/// </summary>
	public bool IsNull => FileID == 0;

	/// <summary>
	/// Unity marked this asset as missing.
	/// </summary>
	public bool IsMissing => Guid == UnityGuid.Missing;

	/// <summary>
	/// The <see cref="PPtr"/> references an internal Unity asset.
	/// </summary>
	public bool IsInternal => Guid == UnityGuid.DefaultResources || Guid == UnityGuid.BuiltinExtra;

	/// <summary>
	/// The <see cref="PPtr"/> references an asset within the same file.
	/// </summary>
	public bool IsIntraFile => Guid == UnityGuid.Zero;

	/// <summary>
	/// Not null, internal, nor missing
	/// </summary>
	public bool IsReplaceable => !IsNull && !IsInternal && !IsMissing;
}
