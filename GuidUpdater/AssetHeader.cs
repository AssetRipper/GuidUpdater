namespace GuidUpdater;

public readonly record struct AssetHeader(int ClassID, long FileID, bool Stripped)
{
}