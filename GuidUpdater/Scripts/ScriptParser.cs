using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace GuidUpdater.Scripts;
public static class ScriptParser
{
	const char NAMESPACE_CLASS_DELIMITER = '.';

	public static bool TryFindPossibleMonoBehaviour(string text, string name, [NotNullWhen(true)] out string? @namespace)
	{
		SyntaxTree syntaxTree = SyntaxFactory.ParseCompilationUnit(text).SyntaxTree;
		SyntaxNode syntaxRoot = syntaxTree.GetRoot();
		foreach (TypeDeclarationSyntax type in syntaxRoot.DescendantNodes().OfType<TypeDeclarationSyntax>())
		{
			AnalyzeType(type, out @namespace, out string nameActual, out bool nested, out bool generic, out bool isClass, out bool isStatic, out bool isInherited, out _);
			if (nameActual == name && !nested && !generic && isClass && !isStatic && isInherited)
			{
				return true;
			}
		}
		@namespace = null;
		return false;
	}

	public static void AnalyzeType(TypeDeclarationSyntax type, out string @namespace, out string name, out bool nested, out bool generic, out bool isClass, out bool isStatic, out bool isInherited, out Visibility visibility)
	{
		GetEncompassingSyntax(type, out LinkedList<BaseNamespaceDeclarationSyntax> namespaces, out LinkedList<TypeDeclarationSyntax> types);
		@namespace = GetNamespace(namespaces);
		name = type.Identifier.Text;
		nested = types.Count > 0;
		generic = GetTotalTypeArgumentCount(type, types) > 0;
		isClass = type is ClassDeclarationSyntax;
		isInherited = type.BaseList is not null && type.BaseList.Types.Any();
		GetModifiers(type, out isStatic, out visibility);
	}

	private static void GetEncompassingSyntax(TypeDeclarationSyntax source, 
		out LinkedList<BaseNamespaceDeclarationSyntax> namespaces, 
		out LinkedList<TypeDeclarationSyntax> types)
	{
		namespaces = new LinkedList<BaseNamespaceDeclarationSyntax>();
		types = new LinkedList<TypeDeclarationSyntax>();
		for (SyntaxNode? parent = source.Parent; parent is not null; parent = parent.Parent)
		{
			if (parent is BaseNamespaceDeclarationSyntax @namespace)
			{
				namespaces.AddFirst(@namespace);
			}
			else if (parent is TypeDeclarationSyntax type)
			{
				types.AddFirst(type);
			}
		}
	}

	private static int GetTypeArgumentCount(TypeDeclarationSyntax type)
	{
		return type.TypeParameterList?.ChildNodes()
			.Count(node => node is TypeParameterSyntax) ?? 0;
	}

	private static int GetTotalTypeArgumentCount(TypeDeclarationSyntax type, LinkedList<TypeDeclarationSyntax> declaringTypes)
	{
		int count = 0;
		foreach (TypeDeclarationSyntax declaringType in declaringTypes)
		{
			count += GetTypeArgumentCount(declaringType);
		}
		count += GetTypeArgumentCount(type);
		return count;
	}

	private static void GetModifiers(TypeDeclarationSyntax type, out bool isStatic, out Visibility visibility)
	{
		bool hasPublicKeyword = false;
		bool hasInternalKeyword = false;
		bool hasPrivateKeyword = false;
		bool hasProtectedKeyword = false;
		isStatic = false;
		for (int i = 0; i < type.Modifiers.Count; i++)
		{
			switch (type.Modifiers[i].ValueText)
			{
				case "public":
					hasPublicKeyword = true;
					break;
				case "internal":
					hasInternalKeyword = true;
					break;
				case "private":
					hasPrivateKeyword = true;
					break;
				case "protected":
					hasProtectedKeyword = true;
					break;
				case "static":
					isStatic = true;
					break;
			}
		}

		if (hasPublicKeyword)
		{
			visibility = Visibility.Public;
		}
		else if (hasProtectedKeyword)
		{
			if (hasInternalKeyword)
			{
				visibility = Visibility.ProtectedOrInternal;
			}
			else if (hasPrivateKeyword)
			{
				visibility = Visibility.ProtectedAndInternal;
			}
			else
			{
				visibility = Visibility.Protected;
			}
		}
		else if (hasPrivateKeyword)
		{
			visibility = Visibility.Private;
		}
		else
		{
			visibility = Visibility.Internal;
		}
	}

	private static string GetNamespace(LinkedList<BaseNamespaceDeclarationSyntax> namespaces)
	{
		if (namespaces.Count <= 1)
		{
			return namespaces.First?.Value.Name.ToString() ?? string.Empty;
		}
		else
		{
			StringBuilder result = new();
			LinkedListNode<BaseNamespaceDeclarationSyntax>? item = namespaces.First;
			result.Append(item!.Value.Name.ToString());
			for (item = item.Next; item is not null; item = item.Next)
			{
				result.Append(NAMESPACE_CLASS_DELIMITER).Append(item.Value.Name.ToString());
			}
			return result.ToString();
		}
	}
}
