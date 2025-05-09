using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SubscriptionDashboard.Views;

public partial class ValidatorCopyContact : UserControl
{
    public static readonly AvaloniaProperty<string?> ContactTextProperty =
        AvaloniaProperty.Register<ValidatorCopyContact, string?>(
            nameof(ContactText));
    
    public string? ContactText 
    {
        get => (string?)GetValue(ContactTextProperty);
        set => SetValue(ContactTextProperty, value);
    }
    
    public static readonly AvaloniaProperty<string?> LabelProperty = 
        AvaloniaProperty.Register<ValidatorCopyContact, string?>(nameof(Label));

    public string? Label
    {
        get => (string?)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }
    
    public ValidatorCopyContact()
    {
        InitializeComponent();
    }
}