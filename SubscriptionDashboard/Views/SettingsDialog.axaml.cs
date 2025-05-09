using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using SubscriptionDashboard.ViewModels;

namespace SubscriptionDashboard.Views;

public partial class SettingsDialog : Window
{
    public SettingsDialog()
    {
        InitializeComponent();
        var vm = new SettingsViewModel();
        vm.CloseAction = Close;
        DataContext = vm;
    }
}