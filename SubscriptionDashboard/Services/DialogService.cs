
using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input.Platform;

namespace SubscriptionDashboard.Services;
public class DialogService
{
    private readonly Window _owner;
    private static DialogService? _instance;

    public static DialogService? Instance => _instance;
    public static void Create(Window window)
    {
        _instance = new DialogService(window);
    }

    private DialogService(Window window)
    {
        _instance = this;
        _owner = window;
    }

    public void ShowDialog(Func<Window> factory) 
    {
        var dialog = factory();
        dialog.ShowDialog(_owner);
    }
}