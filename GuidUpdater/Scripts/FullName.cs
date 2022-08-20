namespace GuidUpdater.Scripts;

public readonly record struct FullName(string Namespace, string Name)
{
	public override string ToString() => string.IsNullOrEmpty(Namespace) ? Name : $"{Namespace}.{Name}";
}