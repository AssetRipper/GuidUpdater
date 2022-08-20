using System;

namespace GuidUpdater;

public readonly record struct UnityGuid(Guid SystemGuid)
{
	public override string ToString() => SystemGuid.ToString("n");

	public static UnityGuid NewGuid() => new UnityGuid(Guid.NewGuid());

	public static UnityGuid Parse(ReadOnlySpan<char> input)
	{
		return new UnityGuid(Guid.ParseExact(input, "n"));
	}

	/// <summary>
	/// Guid composed of all zeros
	/// </summary>
	/// <remarks>
	/// 00000000000000000000000000000000
	/// </remarks>
	public static UnityGuid Zero { get; } = new UnityGuid(Guid.Empty);

	/// <summary>
	/// Guid for missing assets
	/// </summary>
	/// <remarks>
	/// 0000000deadbeef15deadf00d0000000
	/// </remarks>
	public static UnityGuid Missing { get; } = Parse("0000000deadbeef15deadf00d0000000");

	/// <summary>
	/// Guid for "unity default resources"
	/// </summary>
	/// <remarks>
	/// 0000000000000000e000000000000000
	/// </remarks>
	public static UnityGuid DefaultResources { get; } = Parse("0000000000000000e000000000000000");

	/// <summary>
	/// Guid for "unity_builtin_extra"
	/// </summary>
	/// <remarks>
	/// 0000000000000000f000000000000000
	/// </remarks>
	public static UnityGuid BuiltinExtra { get; } = Parse("0000000000000000f000000000000000");
}
