using System.Collections.Generic;

namespace GuidUpdater.Assets;

public readonly record struct NamedObject(UnityAsset Asset, Dictionary<long, UnityAsset> FileDictionary)
{
	public bool IsGameObject => Asset.ClassID is 1;
	public bool IsAnimatorTransition => Asset.ClassID is 1101 or 1109 or 1111;
	public string? Name => Asset.AssetRootNode.GetValue("m_Name").CastToScalar().Value;
	
	public static implicit operator UnityAsset(NamedObject asset) => asset.Asset;
	public static bool IsNamedObject(int classID) => classIDSet.Contains(classID);
	private static readonly HashSet<int> classIDSet = new() //Not a complete set of named object types, only the ones we need for asset matchers
	{
		1,//GameObject
		91,//AnimatorController
		93,//RuntimeAnimatorController
		206,//BlendTree
		240,//AudioMixer
		241,//AudioMixerController
		243,//AudioMixerGroupController
		244,//AudioMixerEffectController
		245,//AudioMixerSnapshotController
		272,//AudioMixerSnapshot
		273,//AudioMixerGroup
		1101,//AnimatorStateTransition
		1102,//AnimatorState
		1107,//AnimatorStateMachine
		1109,//AnimatorTransition
		1111,//AnimatorTransitionBase
	};
}
