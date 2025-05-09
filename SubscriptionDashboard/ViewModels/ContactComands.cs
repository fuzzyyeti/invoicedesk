using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using SubscriptionDashboard.Services;

namespace SubscriptionDashboard.ViewModels;

public class CopyVoteKey : ICommand
{
    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        if (parameter is ValidatorViewModel validator)
        {
            var voteKey = validator.Key;
            Task.Run(async () =>
            {
                await ClipboardService.Instance.SetTextAsync(voteKey);
                NotificationService.Instance.Notify("Copied Vote Key to clipboard");
            });
        }
    }

    public event EventHandler? CanExecuteChanged;
}

public class ContactEmail : ICommand
{
    public bool CanExecute(object? parameter)
    {
        if (parameter is ValidatorViewModel validator
            && DataFetcher.Instance != null
            && DataFetcher.Instance.ValidatorContacts != null
            && DataFetcher.Instance.ValidatorContacts
                .TryGetValue(validator.Key, out var contact))
        {
            return !string.IsNullOrEmpty(contact.XUsername);
        }

        return false;
    }

    public void Execute(object? parameter)
    {
        if (parameter is ValidatorViewModel validator
            && DataFetcher.Instance != null
            && DataFetcher.Instance.ValidatorContacts != null
            && DataFetcher.Instance.ValidatorContacts
                .TryGetValue(validator.Key, out var contact))
        {
            var emailUrl = $"https://mail.google.com/mail/?view=cm&fs=1&to={contact.Email}";
            Process.Start(new ProcessStartInfo
            {
                FileName = emailUrl,
                UseShellExecute = true // Ensures it opens in the default email client
            });
        }
    }

    public event EventHandler? CanExecuteChanged;
}

public class ContactTelegram : ICommand
{
    public bool CanExecute(object? parameter)
    {
        if (parameter is ValidatorViewModel validator
            && DataFetcher.Instance != null
            && DataFetcher.Instance.ValidatorContacts != null
            && DataFetcher.Instance.ValidatorContacts
                .TryGetValue(validator.Key, out var contact))
        {
            return !string.IsNullOrEmpty(contact.TelegramUsername);
        }

        return false;
    }

    public void Execute(object? parameter)
    {
        if (parameter is ValidatorViewModel validator
            && DataFetcher.Instance != null
            && DataFetcher.Instance.ValidatorContacts != null
            && DataFetcher.Instance.ValidatorContacts
                .TryGetValue(validator.Key, out var contact))
        {
            var username = contact.TelegramUsername.AddAt();
            var telegramUrl = $"tg://resolve?domain={username}";
            var fallbackUrl = $"https://web.telegram.org/k/#{username}";

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = telegramUrl,
                UseShellExecute = true
            });
            Task.Run(() =>
            {
                if (process == null) return;
                if (!process.WaitForExit(2000))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = fallbackUrl,
                        UseShellExecute = true
                    });
                }

                ;
                var exitCode = process.ExitCode;
                Console.WriteLine($"Process exited with code: {exitCode}");
                if (exitCode != 0)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = fallbackUrl,
                        UseShellExecute = true
                    });
                }
            });
        }
    }

    public event EventHandler? CanExecuteChanged;
}

public class ContactDiscord : ICommand
{
    public bool CanExecute(object? parameter)
    {
        if (parameter is ValidatorViewModel validator
            && DataFetcher.Instance != null
            && DataFetcher.Instance.ValidatorContacts != null
            && DataFetcher.Instance.ValidatorContacts
                .TryGetValue(validator.Key, out var contact))
        {
            return !string.IsNullOrEmpty(contact.DiscordUsername);
        }

        return false;
    }

    public void Execute(object? parameter)
    {
        if (parameter is ValidatorViewModel validator
            && DataFetcher.Instance != null
            && DataFetcher.Instance.ValidatorContacts != null
            && DataFetcher.Instance.ValidatorContacts
                .TryGetValue(validator.Key, out var contact))
        {
            Task.Run(async () =>
            {
                await ClipboardService.Instance.SetTextAsync(contact.DiscordUsername);
                NotificationService.Instance.Notify("Copied Discord Username to clipboard");
            });
        }
    }

    public event EventHandler? CanExecuteChanged;
}

public class ContactX : ICommand
{
    public bool CanExecute(object? parameter)
    {
        if (parameter is ValidatorViewModel validator
            && DataFetcher.Instance != null
            && DataFetcher.Instance.ValidatorContacts != null
            && DataFetcher.Instance.ValidatorContacts
                .TryGetValue(validator.Key, out var contact))
        {
            return !string.IsNullOrEmpty(contact.XUsername);
        }

        return false;
    }

    public void Execute(object? parameter)
    {
        if (parameter is ValidatorViewModel validator
            && DataFetcher.Instance != null
            && DataFetcher.Instance.ValidatorContacts != null
            && DataFetcher.Instance.ValidatorContacts
                .TryGetValue(validator.Key, out var contact))
        {
            Task.Run(async () =>
            {
                await Task.Run(async () =>
                {
                    await ClipboardService.Instance.SetTextAsync(contact.XUsername);
                    NotificationService.Instance.Notify("Copied Twitter Username to clipboard");
                });
            });
        }
    }

    public event EventHandler? CanExecuteChanged;

    private class TwitterUserResponse
    {
        public TwitterUserData Data { get; set; }
    }

    public class TwitterUserData
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }
    }
}

internal static class DataCleaner
{
    public static string AddAt(this string username)
    {
        return !username.StartsWith("@") ? string.Concat("@", username) : username;
    }
}
