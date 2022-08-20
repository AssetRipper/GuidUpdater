using GuidUpdater.Scripts;

namespace GuidUpdater.Tests;

public class AssemblyParserTests
{
	[Test]
	public void HashWorksForSpline()
	{
		Assert.That(AssemblyParser.ComputeFileID("", "Spline"), Is.EqualTo(194019018));
	}
}