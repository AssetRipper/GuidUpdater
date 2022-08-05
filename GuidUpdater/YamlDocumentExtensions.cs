using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace GuidUpdater;
public static class YamlDocumentExtensions
{
	public static bool TryParseName(this YamlDocument document, [NotNullWhen(true)] out string? name)
	{
		if (document.RootNode is YamlMappingNode mappingNode 
			&& mappingNode.TryGetValue("m_Name", out YamlNode? value)
			&& value is YamlScalarNode valueScalar)
		{
			name = valueScalar.Value ?? string.Empty;
			return true;
		}
		name = null;
		return false;
	}
}
