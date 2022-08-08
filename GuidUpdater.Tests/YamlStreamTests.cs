namespace GuidUpdater.Tests;
internal class YamlStreamTests
{
	[Test]
	public void CorrectlyRecreateMetaFile()
	{
		string metaYaml = @"fileFormatVersion: 2
guid: 12aec0e7dfcb8c64b833494b9e898aa3
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData:
  assetBundleName:
  assetBundleVariant:
".Replace("\r", "");
		MetaFile file = MetaFile.FromText(metaYaml);
		Assert.That(file.Stream.SaveForUnity(false), Is.EqualTo(metaYaml));
	}
}
