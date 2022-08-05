using System.Diagnostics.CodeAnalysis;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;

public static class YamlMappingNodeExtensions
{
	public static bool TryGetValue(this YamlMappingNode node, string key, [NotNullWhen(true)] out YamlNode? value)
	{
		return node.Children.TryGetValue(key, out value);
	}
}