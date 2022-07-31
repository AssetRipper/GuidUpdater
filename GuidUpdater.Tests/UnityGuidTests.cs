namespace GuidUpdater.Tests;

public class UnityGuidTests
{
	[Test]
	public void DefaultGuidString()
	{
		UnityGuid zero = default;
		Assert.Multiple(() =>
		{
			Assert.That(zero, Is.EqualTo(UnityGuid.Zero));
			Assert.That(zero.ToString(), Is.EqualTo("00000000000000000000000000000000"));
		});
	}

	[Test]
	public void ParsingAndToStringAreSymmetrical()
	{
		const string guidString = "882000000e00a000f0003000ddd00010";
		UnityGuid guid = UnityGuid.Parse(guidString);
		Assert.That(guid.ToString(), Is.EqualTo(guidString));
	}
}