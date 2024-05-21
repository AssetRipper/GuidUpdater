using Avalonia;
using Avalonia.Controls;

namespace GuidUpdater.UI;

public partial class CustomToolTip : UserControl
{
	public static readonly StyledProperty<string?> MessageProperty = AvaloniaProperty.Register<CustomToolTip, string?>(nameof(Message), defaultValue: "Test");

	public string? Message
	{
		get => GetValue(MessageProperty);
		set => SetValue(MessageProperty, value);
	}

	public CustomToolTip()
	{
		InitializeComponent();
		((StyledElement?)Content)!.DataContext = this;
	}
}