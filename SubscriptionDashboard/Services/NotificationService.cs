using Avalonia.Controls;
using Avalonia.Controls.Notifications;

namespace SubscriptionDashboard.Services;

public class NotificationService
{
    private readonly WindowNotificationManager _notifcationManager;

    public static void Create(Window window)
    {
        Instance = new NotificationService(window);
        
    }
    
    public static NotificationService Instance { get; private set; } = null!;

    public void Notify(string message)
    {
        Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
        {
            _notifcationManager.Show(new Notification("Notification", message));
        });
    }
    
    private NotificationService(Window window)
    {
        _notifcationManager = new WindowNotificationManager(window)
        {
            Position = NotificationPosition.BottomCenter,
            MaxItems = 3
        };
    }
}