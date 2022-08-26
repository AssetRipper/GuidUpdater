using AsmResolver.DotNet;
using System.Collections.Generic;
using System.Text;

namespace GuidUpdater.Scripts;

public static class AssemblyParser
{
	private static readonly HashSet<string> systemBaseTypes = new()
	{
		"System.Object",
		"System.Enum",
		"System.ValueType",
		"System.Exception",
		"System.Attribute",
	};

	public static HashSet<FullName> GetPotentialMonoBehavioursFromAssembly(string path)
	{
		HashSet<FullName> result = new();
		ModuleDefinition module = ModuleDefinition.FromFile(path);
		foreach (TypeDefinition type in module.TopLevelTypes)
		{
			if (type.IsClass && !type.IsAbstract && type.GenericParameters.Count == 0 && IsValidBaseType(type.BaseType))
			{
				string @namespace = type.Namespace?.Value ?? "";
				string name = type.Name?.Value ?? "";
				result.Add(new FullName(@namespace, name));
			}
		}
		return result;
	}

	private static bool IsValidBaseType(ITypeDefOrRef? baseType)
	{
		return baseType is not null && !systemBaseTypes.Contains(baseType.FullName.ToString());
	}

	public static int ComputeFileID(FullName fullName) => ComputeFileID(fullName.Namespace, fullName.Name);
	public static int ComputeFileID(string @namespace, string name)
	{
		string toBeHashed = $"s\0\0\0{@namespace}{name}";
		using MD4 hash = new();
		byte[] hashed = hash.ComputeHash(Encoding.UTF8.GetBytes(toBeHashed));

		int result = 0;
		for (int i = 3; i >= 0; --i)
		{
			result <<= 8;
			result |= hashed[i];
		}

		return result;
	}
}