using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;

public static class TestClass
{
	static string transformYaml = @"
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {fileID: 0}
  m_PrefabInstance: {fileID: 0}
  m_PrefabAsset: {fileID: 0}
  m_GameObject: {fileID: 5685754235442032086}
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_Children:
  - {fileID: 5685754235442028800}
  m_Father: {fileID: 5685754235442028801}
  m_RootOrder: 0
  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}
";
	static string metaYaml = @"
fileFormatVersion: 2
guid: 12aec0e7dfcb8c64b833494b9e898aa3
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
";
	internal static void Whatever()
	{
		//AnalyzeYaml(metaYaml);
		string guid = GetGuid(metaYaml);
		Console.WriteLine(guid.Length);
		Console.WriteLine(guid);
	}

	private static string GetGuid(string yamlText)
	{
		YamlDocument document = GetYamlDocuments(yamlText).Single();
		YamlMappingNode rootNode = document.RootNode as YamlMappingNode ?? throw new Exception("Root node must be a map");
		YamlScalarNode guidNode = (YamlScalarNode)rootNode.Children.Single(pair => ((YamlScalarNode)pair.Key).Value == "guid").Value;
		return guidNode.Value ?? throw new Exception("Guid cannot be null");
	}

	private static void AnalyzeYaml(string yamlText)
	{
		IList<YamlDocument> documents = GetYamlDocuments(yamlText);
		foreach (YamlDocument document in documents)
		{
			PrintDocument(document);
		}
	}

	internal static void PrintDocument(YamlDocument document)
	{
		YamlNode rootNode = document.RootNode;
		PrintNode(rootNode, 0);
	}

	private static void PrintNode(YamlNode node, int indent)
	{
		switch (node)
		{
			case YamlScalarNode scalarNode:
				Console.WriteLine($"{GetIndent(indent)}Scalar Node: {scalarNode.Value}");
				return;
			case YamlSequenceNode sequenceNode:
				Console.WriteLine($"{GetIndent(indent)}Sequence Node: {sequenceNode.Children.Count}");
				for (int i = 0; i < sequenceNode.Children.Count; i++)
				{
					var child = sequenceNode.Children[i];
					PrintNode(child, indent + 1);
				}
				return;
			case YamlMappingNode mappingNode:
				Console.WriteLine($"{GetIndent(indent)}Mapping Node: {mappingNode.Children.Count}");
				for (int i = 0; i < mappingNode.Children.Count; i++)
				{
					var pair = mappingNode.Children[i];
					Console.WriteLine($"{GetIndent(indent + 1)}Pair {i}");
					PrintNode(pair.Key, indent + 2);
					PrintNode(pair.Value, indent + 2);
				}
				return;
		}
	}

	private static string GetIndent(int count)
	{
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < count; i++)
		{
			sb.Append('\t');
		}
		return sb.ToString();
	}

	internal static IList<YamlDocument> GetYamlDocuments(string yamlText)
	{
		var yaml = new YamlStream();
		using var fs = new StringReader(yamlText);
		yaml.Load(fs);
		return yaml.Documents;
	}
}
