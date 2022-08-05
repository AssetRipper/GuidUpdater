namespace GuidUpdater.Tests;
public class YamlMetaParserTests
{
	[Test]
	public void CorrectlyParseGuid()
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
		UnityGuid guid = YamlMetaParser.GetGuidFromYamlStream(YamlLoader.LoadMetaYamlStreamFromText(metaYaml));
		Assert.That(guid.ToString(), Is.EqualTo("12aec0e7dfcb8c64b833494b9e898aa3"));
	}
}
