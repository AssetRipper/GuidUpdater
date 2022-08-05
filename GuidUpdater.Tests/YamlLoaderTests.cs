using YamlDotNet.RepresentationModel;

namespace GuidUpdater.Tests;

internal class YamlLoaderTests
{
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
  serializedVersion: 6";
		IList<YamlDocument> documents = YamlLoader.LoadAssetYamlStreamFromText(yamlText).Documents;
		Assert.That(documents.Count, Is.EqualTo(2));
		Assert.That(documents[1].RootNode.Anchor.Value, Is.EqualTo("2100001 stripped"));
	}
}
