namespace GuidUpdater.Tests;
internal class YamlStreamTests
{
	const string metaYaml = @"fileFormatVersion: 2
guid: 12aec0e7dfcb8c64b833494b9e898aa3
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData:
  assetBundleName:
  assetBundleVariant:
";
	[Test]
	public void CorrectlyRecreateMetaFile()
	{
		MetaFile file = MetaFile.FromText(metaYaml);
		Assert.That(file.Stream.SaveForUnity(), Is.EqualTo(metaYaml));
	}
}
