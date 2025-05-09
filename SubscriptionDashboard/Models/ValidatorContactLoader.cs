using System.Collections.Generic;
using System.IO;
using SubscriptionDashboard.Services;

namespace SubscriptionDashboard.Models;

public class ValidatorContactLoader
{

    public Dictionary<string, ValidatorContact> LoadValidatorContacts()
    {
        var filePath = ConfigurationService.Instance?.Configuration.ValidatorContacts;
        var contacts = new Dictionary<string, ValidatorContact>();

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Validator contacts file not found at {filePath}");

        var lines = File.ReadAllLines(filePath);
        for (int i = 1; i < lines.Length; i++) // Skip header
        {
            var columns = lines[i].Split(',');

            if (columns.Length < 8) continue; // Skip invalid rows

            var voteKey = columns[4].Trim();
            if (!string.IsNullOrEmpty(voteKey))
            {
                contacts[voteKey] = new ValidatorContact
                {
                    DiscordUsername = columns[2].Trim(),
                    TelegramUsername = columns[3].Trim(),
                    XUsername = columns[7].Trim()
                };
            }
        }

        return contacts;
    }
}

public class ValidatorContact
{
    public string DiscordUsername { get; set; } = string.Empty;
    public string TelegramUsername { get; set; } = string.Empty;
    public string XUsername { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}