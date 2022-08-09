using System.Collections.Generic;
using System.IO;

namespace GuidUpdater;
internal static class FilePaths
{
	private static HashSet<string> IgnoredFolders { get; } = new()
	{
		"CombinedMesh",//Combined meshes have conflicting names
	};
	private static HashSet<string> IgnoredFileExtensions { get; } = new()
	{
		".cs",
		".dll",
		".shader",
		".png",
		".obj",
		".fbx",
		".blend",
		".unity3d",
		".otf",
		".ttf",
		".bytes",
		".txt",
		".json",
	};

	public static bool IsSerializedFile(string path)
	{
		string extension = Path.GetExtension(path);
		return !IgnoredFileExtensions.Contains(extension);
	}

	public static bool IsSerializedFileAndNotScene(string path)
	{
		string extension = Path.GetExtension(path);
		return !IgnoredFileExtensions.Contains(extension) && extension != ".unity";
	}

	public static bool IsIgnoredFolder(string directory)
	{
		return IgnoredFolders.Contains(Path.GetFileName(directory));
	}
}
