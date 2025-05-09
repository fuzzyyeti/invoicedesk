using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SubscriptionDashboard.Models;
using SubscriptionDashboard.Services;

namespace SubscriptionDashboard.ViewModels;

public class ValidatorViewModel : ViewModelBase
{

    private readonly Validator _validator;


    public ValidatorViewModel(Validator validator)
    {
        _validator = validator;
        SelectCommand = new RelayCommand(OnSelect);
        ShortKey = new string(_validator.VoteKey.AsSpan()[..4]) + "..." + new string(_validator.VoteKey.AsSpan()[^4..]);
        if (DataFetcher.Instance?.ValidatorInfos.TryGetValue(_validator.VoteKey, out var voteInfo) == true
            && voteInfo is { Name: { } name })
        {
            Name = name;
        }
        else
        {
            Name = ShortKey;
        }
        UnpaidInvoicesCount = _validator.Invoices.Count(x => x.BalanceOutstanding > 0); 
        StatusColor = UnpaidInvoicesCount switch
        {
            < 3 => new LinearGradientBrush
                {
                    GradientStops = new GradientStops
                    {
                        new GradientStop(Color.Parse("#A8E6CF"), 0), // Soft mint green
                        new GradientStop(Color.Parse("#56C596"), 1)  // Richer green
                    }
                },
                < 6 => new LinearGradientBrush
            {
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#FFECB3"), 0), // Soft pastel yellow
                    new GradientStop(Color.Parse("#FFC107"), 1)  // Vibrant amber
                }
            },
            _ => new LinearGradientBrush
            {
                GradientStops = new GradientStops
                {
                    new GradientStop(Color.Parse("#FFABAB"), 0), // Soft coral pink
                    new GradientStop(Color.Parse("#FF5252"), 1)  // Bold red
                }
            }
        };
        if (DataFetcher.Instance?.ValidatorContacts?.TryGetValue(_validator.VoteKey, out var contact) == true)
        {
            Contact = contact;
        }
        
        if (DataFetcher.Instance?.ValidatorInfos.TryGetValue(_validator.VoteKey, out voteInfo) == true 
            && voteInfo is { Icon: { } iconPath })
        {
            Icon = new Bitmap(AssetLoader.Open(new Uri($"avares://SubscriptionDashboard/Assets/ValidatorIcons{iconPath}")));
        }
    }
    
    private void OnSelect()
    {
        Selected?.Invoke(this, EventArgs.Empty);
        OnPropertyChanged(nameof(Name));
    }
    
    public RelayCommand SelectCommand { get; }

    public event EventHandler<EventArgs>? Selected;
    
    public IBrush StatusColor { get; }

    public int UnpaidInvoicesCount { get ; }

    public string ShortKey { get; }

    public string Key => _validator.VoteKey;
    
    public IEnumerable<Invoice> Invoices => _validator.Invoices;

    public string Name { get; }

    public Bitmap Icon { get; }
    
    public ContactDiscord ContactDiscord => new ();
    
    public ContactTelegram ContactTelegram => new ();

    public ValidatorContact? Contact { get; }
    
    public ContactX ContactX => new();
    public ContactEmail ContactEmail => new();
    
    public CopyVoteKey CopyVoteKey => new();
    
    public RelayCommand<string> CopyCommand => new (OnCopy);
    
    private void OnCopy(string  copyText)
    {
        Task.Run(async () =>
        {
            await ClipboardService.Instance.SetTextAsync(copyText);
            NotificationService.Instance.Notify($"Copied {copyText} to clipboard");
        });
    }
        
}