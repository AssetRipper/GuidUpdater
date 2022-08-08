using HarmonyLib;
using System;
using System.Reflection;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Core.Tokens;
using DocumentStart = YamlDotNet.Core.Events.DocumentStart;

namespace GuidUpdater;

internal static class EmitterPatch
{
	private static readonly FieldInfo scalarDataField;
	private static readonly Type scalarDataType;
	private static readonly FieldInfo scalarDataValueField;
	private static readonly FieldInfo tagDirectivesField;
	private static readonly MethodInfo processTagMethod;
	private static readonly MethodInfo analyzeTagMethod;
	private static readonly Harmony harmony = new Harmony("GuidUpdater");
	private static readonly TagDirective unityTag = new TagDirective("!u!", "tag:unity3d.com,2011:");
	public static bool EmittingAssetFile { get; set; }

	static EmitterPatch()
	{
		scalarDataField = typeof(Emitter).GetField("scalarData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new Exception("Could not find Emitter.scalarData");
		tagDirectivesField = typeof(Emitter).GetField("tagDirectives", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new Exception("Could not find Emitter.tagDirectives");
		scalarDataType = typeof(Emitter).GetNestedType("ScalarData", BindingFlags.NonPublic) ?? throw new Exception("Could not find ScalarData");
		scalarDataValueField = scalarDataType.GetField("Value", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new Exception("Could not find ScalarData.Value");
		processTagMethod = typeof(Emitter).GetMethod("ProcessTag", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new Exception("Could not find Emitter.ProcessTag()");
		analyzeTagMethod = typeof(Emitter).GetMethod("AnalyzeTag", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new Exception("Could not find Emitter.ProcessTag()");
	}

	private static string? GetScalarValue(Emitter emitter)
	{
		object scalarData = GetScalarData(emitter);
		object? value = scalarDataValueField.GetValue(scalarData);
		return (string?)value;
	}

	private static object GetScalarData(Emitter emitter)
	{
		return scalarDataField.GetValue(emitter) ?? throw new NullReferenceException("scalarData");
	}

	[HarmonyPatch(typeof(Emitter), "ProcessScalar")]
	[HarmonyPrefix]
	private static bool EmptyScalarsShouldBePlainStyle(Emitter __instance)
	{
		return !string.IsNullOrEmpty(GetScalarValue(__instance));
	}

	[HarmonyPatch(typeof(Emitter), "AppendTagDirectiveTo")]
	[HarmonyPrefix]
	private static bool OnlyAppendUnityTag(TagDirective value)
	{
		return value == unityTag;
	}
	
	[HarmonyPatch(typeof(Emitter), "EmitDocumentStart")]
	[HarmonyPrefix]
	private static void AddTagAndVersionToHeader(Emitter __instance, ref ParsingEvent evt, bool isFirst)
	{
		if (EmittingAssetFile && evt is DocumentStart originalEvent)
		{
			if (isFirst)
			{
				VersionDirective version = new VersionDirective(new YamlDotNet.Core.Version(1, 1));
				TagDirectiveCollection tags = new TagDirectiveCollection();
				tags.Add(unityTag);
				evt = new DocumentStart(version, tags, false, originalEvent.Start, originalEvent.End);
			}
			else
			{
				TagDirectiveCollection tagDirectives = (TagDirectiveCollection?)tagDirectivesField.GetValue(__instance) ?? throw new Exception();
				tagDirectives.Add(unityTag);
			}
		}
	}

	[HarmonyPatch(typeof(Emitter), "AnalyzeEvent")]
	[HarmonyPrefix]
	private static void EnsureClassIdIsEmitted(Emitter __instance, ParsingEvent evt)
	{
		if (evt is NodeEvent nodeEvent && !nodeEvent.Tag.IsEmpty)
		{
			analyzeTagMethod.Invoke(__instance, new object[] { nodeEvent.Tag });
			processTagMethod.Invoke(__instance, null);
		}
	}

	[HarmonyPatch(typeof(YamlDotNet.Core.Events.DocumentEnd), MethodType.Constructor, typeof(bool), typeof(Mark), typeof(Mark))]
	[HarmonyPrefix]
	private static void DocumentShouldEndImplicit(ref bool isImplicit)
	{
		isImplicit = true;
	}

	internal static void Apply()
	{
		harmony.PatchAll(typeof(EmitterPatch));
	}

	internal static void Remove()
	{
		harmony.UnpatchSelf();
	}
}