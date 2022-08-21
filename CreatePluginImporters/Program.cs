using GuidUpdater;

namespace CreatePluginImporters;

internal class Program
{
	static void Main(string[] args)
	{
		CreateMultipleImporters(args[0]);
		Console.WriteLine("Done!");
	}

	static void CreateMultipleImporters(string directory)
	{
		foreach (string path in Directory.EnumerateFiles(directory, "*.dll"))
		{
			CreateRandomPluginImporter($"{path}.meta");
		}
	}

	static void CreateRandomPluginImporter(string path)
	{
		UnityGuid guid = UnityGuid.NewGuid();
		string text = $@"fileFormatVersion: 2
guid: {guid}
PluginImporter:
  externalObjects: {{}}
  serializedVersion: 2
  iconMap: {{}}
  executionOrder: {{}}
  defineConstraints: []
  isPreloaded: 0
  isOverridable: 0
  isExplicitlyReferenced: 0
  validateReferences: 1
  platformData:
  - first:
      Any: 
    second:
      enabled: 1
      settings: {{}}
  - first:
      Editor: Editor
    second:
      enabled: 0
      settings:
        DefaultValueInitialized: true
  - first:
      Windows Store Apps: WindowsStoreApps
    second:
      enabled: 0
      settings:
        CPU: AnyCPU
  userData: 
  assetBundleName: 
  assetBundleVariant: 
";
		File.WriteAllText(path, text);
	}
}
