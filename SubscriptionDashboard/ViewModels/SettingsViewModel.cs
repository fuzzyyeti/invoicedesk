using System;
using System.Windows.Input;
using SubscriptionDashboard.Services;

namespace SubscriptionDashboard.ViewModels;

public class SettingsViewModel
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

    public ICommand UpdateConfiuration => new RelayCommand(UpdateJsonFile);

    public ICommand CloseCommand => new RelayCommand(CloseAction!);

    private void UpdateJsonFile()
    {
        if (_settings != null)
        {
            ConfigurationService.Instance?.UpdateSettings(_settings);
        }
        CloseAction?.Invoke();
    }
}
