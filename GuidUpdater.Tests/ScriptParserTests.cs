using GuidUpdater.Scripts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GuidUpdater.Tests;
internal class ScriptParserTests
{
	const string code = @"

			using System;

			namespace TestProgram {
				namespace NestedNamespace {
					class Program<T> {
						public void My() { var i = 5; do { Console.WriteLine(""hello world""); i++; } while (i > 10); }
						private struct NestedStruct { }
					}
				}
				internal static class InternalStaticClass { }
				public struct PublicStruct { }
			}
			public class NormalMonoBehaviour : MonoBehaviour { }
			";

	[Test]
	public void AnalyzeCorrectly()
	{
		SyntaxTree syntaxTree = SyntaxFactory.ParseCompilationUnit(code).SyntaxTree;
		SyntaxNode syntaxRoot = syntaxTree.GetRoot();
		TypeDeclarationSyntax[] declarations = syntaxRoot.DescendantNodes().OfType<TypeDeclarationSyntax>().ToArray();
		AssertCorrectAnalysis(declarations[0], "TestProgram.NestedNamespace", "Program", false, true, true, false, false, Visibility.Internal);
		AssertCorrectAnalysis(declarations[1], "TestProgram.NestedNamespace", "NestedStruct", true, true, false, false, false, Visibility.Private);
		AssertCorrectAnalysis(declarations[2], "TestProgram", "InternalStaticClass", false, false, true, true, false, Visibility.Internal);
		AssertCorrectAnalysis(declarations[3], "TestProgram", "PublicStruct", false, false, false, false, false, Visibility.Public);
		AssertCorrectAnalysis(declarations[4], "", "NormalMonoBehaviour", false, false, true, false, true, Visibility.Public);
	}

	private static void AssertCorrectAnalysis(TypeDeclarationSyntax declaration, string namespaceE, string nameE, bool nestedE, bool genericE, bool isClassE, bool isStaticE, bool isInheritedE, Visibility visibilityE)
	{
		ScriptParser.AnalyzeType(declaration, out string namespaceA, out string nameA, out bool nestedA, out bool genericA, out bool isClassA, out bool isStaticA, out bool isInheritedA, out Visibility visibilityA);
		Assert.Multiple(() =>
		{
			Assert.That(namespaceA, Is.EqualTo(namespaceE));
			Assert.That(nameA, Is.EqualTo(nameE));
			Assert.That(nestedA, Is.EqualTo(nestedE));
			Assert.That(genericA, Is.EqualTo(genericE));
			Assert.That(isClassA, Is.EqualTo(isClassE));
			Assert.That(isStaticA, Is.EqualTo(isStaticE));
			Assert.That(isInheritedA, Is.EqualTo(isInheritedE));
			Assert.That(visibilityA, Is.EqualTo(visibilityE));
		});
	}

	[Test]
	public void FindsTheMonoBehaviour()
	{
		Assert.That(ScriptParser.TryFindPossibleMonoBehaviour(code, "NormalMonoBehaviour", out string? @namespace));
		Assert.That(@namespace, Is.EqualTo(""));
	}
}
