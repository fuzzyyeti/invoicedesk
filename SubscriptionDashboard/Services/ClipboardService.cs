
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input.Platform;

namespace SubscriptionDashboard.Services;

public class ClipboardService
{
   private static ClipboardService? _instance;
   private readonly IClipboard? _clipboard;

    public static void Create(Window window)
    {
         _instance = new ClipboardService(window);
    }

   private ClipboardService(Window window)
   {
       _clipboard = window.Clipboard;
   }

   public static ClipboardService Instance => _instance!;
   
   public async Task SetTextAsync(string text)
   {
       await _clipboard?.SetTextAsync(text)!;
   }
}