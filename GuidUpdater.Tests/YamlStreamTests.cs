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

	[Test]
	public void FileWithStrippedAssetsParseWithoutErrors()
	{
		string yamlText = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!21 &2100000
Material:
  serializedVersion: 6
--- !u!22 &2100001 stripped
Material:
  serializedVersion: 6
".Replace("\r", "");
		AssetFile file = AssetFile.FromText(yamlText);
		Assert.That(file.Stream.SaveForUnity(true), Is.EqualTo(yamlText));
	}
}
