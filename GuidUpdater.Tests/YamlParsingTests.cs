using YamlDotNet.RepresentationModel;

namespace GuidUpdater.Tests;
internal class YamlParsingTests
{
	private const string PrefabText = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!1 &3975326226355896
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  serializedVersion: 6
  m_Component:
  - component: {fileID: 3975326226366417}
  m_Layer: 0
  m_Name: Dummy_Prefab
  m_TagString: Untagged
  m_Icon: {fileID: 0}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &3975326226366417
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 3975326226355896}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_Children: []
  m_Father: {fileID: 0}
  m_RootOrder: 0
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}";
	private const long GameObjectFileID = 3975326226355896;
	private const long TransformFileID = 3975326226366417;
	
	[Test]
	public void ParseGameObject()
	{
		YamlStream stream = AssetFile.FromText(PrefabText);
		UnityAsset asset = stream.Documents[0];
		Assert.Multiple(() =>
		{
			Assert.That(asset.ClassID, Is.EqualTo(1));
			Assert.That(asset.FileID, Is.EqualTo(GameObjectFileID));
			Assert.That(asset.Stripped, Is.EqualTo(false));
		});
		Assert.That(asset.TryParseName(out string? name));
		Assert.That(name, Is.EqualTo("Dummy_Prefab"));
	}
	
	[Test]
	public void ParseTransform()
	{
		YamlStream stream = AssetFile.FromText(PrefabText);
		UnityAsset asset = stream.Documents[1];
		Assert.Multiple(() =>
		{
			Assert.That(asset.ClassID, Is.EqualTo(4));
			Assert.That(asset.FileID, Is.EqualTo(TransformFileID));
			Assert.That(asset.Stripped, Is.EqualTo(false));
		});
	}
}