using System.Linq;
using System.Resources;
using Avalonia.Controls;
using Avalonia.Input;
using SubscriptionDashboard.Models;
using SubscriptionDashboard.Services;
using SubscriptionDashboard.ViewModels;

namespace SubscriptionDashboard.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataFetcher.Instance?.RefreshData();
        ClipboardService.Create(this);
        NotificationService.Create(this);
        DialogService.Create(this);
    }
}