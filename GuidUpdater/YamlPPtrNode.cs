using System.Diagnostics;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;
public readonly record struct YamlPPtrNode(YamlMappingNode RootNode, bool IntraFile)
{
	public long FileID
	{
		get => long.Parse(FileIDValueNode.Value!);
		set => FileIDValueNode.Value = value.ToString();
	}

	public UnityGuid Guid
	{
		get
		{
			return IntraFile ? UnityGuid.Zero : UnityGuid.Parse(GuidValueNode.Value!);
		}
		set
		{
			if (!IntraFile)
			{
				GuidValueNode.Value = value.ToString();
			}
		}
	}

	public AssetType Type
	{
		get
		{
			return IntraFile ? AssetType.Serialized : (AssetType)byte.Parse(TypeValueNode.Value!);
		}
		set
		{
			if (!IntraFile)
			{
				TypeValueNode.Value = ((byte)value).ToString();
			}
		}
	}

	private YamlScalarNode FileIDValueNode
	{
		get
		{
			Debug.Assert(RootNode.GetChild(0).Key.Value == "fileID");
			return (YamlScalarNode)RootNode.GetChild(0).Value;
		}
	}

	private YamlScalarNode GuidValueNode
	{
		get
		{
			Debug.Assert(!IntraFile);
			Debug.Assert(RootNode.Children.Count == 3);
			Debug.Assert(RootNode.GetChild(1).Key.Value == "guid");
			return (YamlScalarNode)RootNode.GetChild(1).Value;
		}
	}

	private YamlScalarNode TypeValueNode
	{
		get
		{
			Debug.Assert(!IntraFile);
			Debug.Assert(RootNode.Children.Count == 3);
			Debug.Assert(RootNode.GetChild(2).Key.Value == "type");
			return (YamlScalarNode)RootNode.GetChild(2).Value;
		}
	}

	public PPtr PPtr
	{
		get
		{
			return new PPtr(FileID, Guid, Type);
		}
		set
		{
			FileID = value.FileID;
			Guid = value.Guid;
			Type = value.Type;
		}
	}
}
