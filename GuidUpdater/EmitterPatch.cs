using HarmonyLib;
using System;
using System.Collections.Generic;
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
	private static readonly Harmony harmony = new Harmony("GuidUpdater");
	public static bool EmittingAssetFile { get; set; }

	static EmitterPatch()
	{
		scalarDataField = typeof(Emitter).GetField("scalarData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new Exception("Could not find Emitter.scalarData");
		scalarDataType = typeof(Emitter).GetNestedType("ScalarData", BindingFlags.NonPublic) ?? throw new Exception("Could not find ScalarData");
		scalarDataValueField = scalarDataType.GetField("Value", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new Exception("Could not find ScalarData.Value");
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
	private static bool AppendTagDirectiveTo()
	{
		return false;
	}

	[HarmonyPatch(typeof(Emitter), "NonDefaultTagsAmong")]
	[HarmonyPrefix]
	private static bool NonDefaultTagsAmong(IEnumerable<TagDirective>? tagCollection, ref TagDirectiveCollection __result)
	{
		if (tagCollection is not null && tagCollection is TagDirectiveCollection collection)
		{
			__result = collection;
			return false;
		}
		return true;
	}

	[HarmonyPatch(typeof(Emitter), "EmitDocumentStart")]
	[HarmonyPrefix]
	private static void AddTagAndVersionToHeader(ref ParsingEvent evt, bool isFirst)
	{
		if (isFirst && EmittingAssetFile)
		{
			DocumentStart start = (DocumentStart)evt;
			VersionDirective version = new VersionDirective(new YamlDotNet.Core.Version(1, 1));
			TagDirectiveCollection tags = new TagDirectiveCollection();
			TagDirective tag = new TagDirective("!u!", "tag:unity3d.com,2011:");
			tags.Add(tag);
			evt = new DocumentStart(version, tags, false, start.Start, start.End);
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