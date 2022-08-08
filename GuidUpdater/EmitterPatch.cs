﻿using HarmonyLib;
using System;
using System.Reflection;
using YamlDotNet.Core;

namespace GuidUpdater;

internal static class EmitterPatch
{
	private static readonly FieldInfo scalarDataField;
	private static readonly Type scalarDataType;
	private static readonly FieldInfo scalarDataValueField;
	private static readonly Harmony harmony = new Harmony("GuidUpdater");

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

	internal static void Apply()
	{
		harmony.PatchAll(typeof(EmitterPatch));
	}

	internal static void Remove()
	{
		harmony.UnpatchSelf();
	}
}