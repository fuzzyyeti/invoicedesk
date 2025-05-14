using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using SubscriptionDashboard.Services;

namespace SubscriptionDashboard.ViewModels;

public partial class SettingsViewModel
{
    private readonly Settings? _settings = ConfigurationService.Instance?.Configuration;

    public Action? CloseAction { get; set; }
    public string? ApiUrl
    {
        get => _settings?.InvoiceApiUrl;
        set
        {
            if (value != null && _settings != null)
            {
                _settings.InvoiceApiUrl = value;
            }
        }
    }
    public string? ValidatorContacts
    {
        get => _settings?.ValidatorContacts;
        set
        {
            if (value != null && _settings != null)
            {
                _settings.ValidatorContacts = value;
            }
        }
    }

    [RelayCommand]
    public void CloseCommand()
    {
        CloseAction?.Invoke();
    }

    [RelayCommand]
    public void UpdateConfiguration()
    {
        if (_settings != null)
        {
            ConfigurationService.Instance?.UpdateSettings(_settings);
        }
        CloseAction?.Invoke();
    }
}
