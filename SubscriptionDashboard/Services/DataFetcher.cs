using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SubscriptionDashboard.Models;

namespace SubscriptionDashboard.Services;

public class DataFetcher
{
    private static DataFetcher? _instance;
    
    public event Action? DataUpdated;
    
    public static DataFetcher Instance => 
        LazyInitializer.EnsureInitialized(ref _instance, () => new DataFetcher());

    public Dictionary<int, float> EpochPercentages { get; private set; } = new();

    public Dictionary<string, ValidatorInfo>? ValidatorInfos { get; private set; }
    
    public Dictionary<string, ValidatorContact>? ValidatorContacts { get; private set; }

    public void RefreshData()
    {
        Task.Run(async () =>
        {
            Validators = [];
            Avalonia.Threading.Dispatcher.UIThread.Post(() => { DataUpdated?.Invoke(); });
            var apiUrl = ConfigurationService.Instance?.Configuration.InvoiceApiUrl;
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(apiUrl);
            var invoices = JsonSerializer.Deserialize<List<Invoice>>(response,
                new JsonSerializerOptions() {PropertyNameCaseInsensitive = true});
            if (invoices != null)
            {
                Validators = invoices.Where(i => i.Epoch > 769)
                    .GroupBy(
                        invoice => invoice.VoteAccount,
                        invoice => invoice,
                        (key, group) => new Validator
                        {
                            VoteKey = key,
                            Invoices = group.ToArray()
                        }).ToArray();

                EpochPercentages = invoices.Where(i => i.Epoch > 769)
                    .GroupBy(
                        invoice => invoice.Epoch,
                        invoice => invoice.BalanceOutstanding,
                        (key, group) =>
                            new KeyValuePair<int, float>(key, (float)group.Count(i => i == 0) / group.Count()))
                    .ToDictionary();
                if (ValidatorInfos == null)
                {
                    ValidatorInfos = new Dictionary<string, ValidatorInfo>();
                    var validatorInfos = File.ReadAllText("validatorInfos.json"); 
                    var validatorInfoLayouts = JsonSerializer.Deserialize<ValidatorInfoLayout[]>(validatorInfos,
                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true});
                    if (validatorInfoLayouts != null)
                    {
                        foreach (var validatorInfoLayout in validatorInfoLayouts)
                        {
                            ValidatorInfos[validatorInfoLayout.VoteAccountPubkey] = new ValidatorInfo
                            {
                                Name = validatorInfoLayout.Name.Trim(),
                                Icon = validatorInfoLayout.Icon
                            };
                        }
                    }
                }
                if (ValidatorContacts == null)
                {
                    try
                    {
                        var validatorContactLoader = new ValidatorContactLoader();
                        ValidatorContacts = validatorContactLoader.LoadValidatorContacts();
                    }
                    catch (FileNotFoundException e)
                    {
                        ValidatorContacts = new();
                    }
                }
                Avalonia.Threading.Dispatcher.UIThread.Post(() => { DataUpdated?.Invoke(); });
            }
        });
    }
    
    public Validator[]? Validators { get; private set; }    
    private class ValidatorInfoLayout
    {
        public string VoteAccountPubkey { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}
