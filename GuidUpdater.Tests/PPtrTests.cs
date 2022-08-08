namespace GuidUpdater.Tests;

public class PPtrTests
{
	[Test]
	public void DefaultPointerString()
	{
		PPtr pptr = default;
		Assert.That(pptr.ToString(), Is.EqualTo("{fileID: 0}"));
	}

	[Test]
	public void InternalPointerString()
	{
		PPtr pptr = new PPtr(10304, UnityGuid.BuiltinExtra, AssetType.Internal);
		Assert.That(pptr.ToString(), Is.EqualTo("{fileID: 10304, guid: 0000000000000000f000000000000000, type: 0}"));
	}
}
