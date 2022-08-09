namespace GuidUpdater.Tests;
public class YamlMetaParserTests
{
	[Test]
	public void CorrectlyParseMetaFile()
	{
		string metaYaml = @"
fileFormatVersion: 2
guid: 12aec0e7dfcb8c64b833494b9e898aa3
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData:
  assetBundleName:
  assetBundleVariant:
";
		MetaFile file = MetaFile.FromText(metaYaml);
		Assert.Multiple(() =>
		{
			Assert.That(file.Guid.ToString(), Is.EqualTo("12aec0e7dfcb8c64b833494b9e898aa3"));
			Assert.That(file.ImporterName, Is.EqualTo("DefaultImporter"));
			Assert.That(file.ImporterRootNode.Children, Has.Count.EqualTo(4));
		});
	}
}
